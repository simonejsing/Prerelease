using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using CraftingGame.Controllers;
using CraftingGame.Physics;
using CraftingGame.Widgets;
using Terrain;
using VectorMath;
using CraftingGame.Actions;
using CraftingGame.State;
using System.IO;

namespace CraftingGame
{
    public class PlatformerSceene : Sceene
    {
        public const int TerrainDepth = 100;
        public const int TerrainHeight = 100;
        public const int TerrainSeaLevel = 80;
        public const int BlockSize = 30;

        // Use a fixed update step size.
        private const float UpdateStep = 1.0f;

        private readonly IStreamProvider streamProvider;
        private readonly ActionQueue actionQueue;
        private readonly ITerrainFactory terrainFactory;

        private SpriteResolver spriteResolver;
        private PhysicsEngine physics;
        private IFont debugFont = null;
        private InputMask[] inputMasks;

        // Terrain
        private ProceduralLevel level;

        // Widgets
        private TerrainWidget terrainWidget;
        private DynamicGridWidget dynamicGridWidget;

        // Actions
        private CollectAction collectAction;

        // Input controllers
        private FreeCameraController freeCameraController;
        private PlayerController playerController;

        public View[] ActiveViews { get; private set; }
        public Camera FirstCamera => ActiveViews.First().Camera;

        public IModifiableTerrain Terrain => State.Terrain;
        public ViewportProjection DisplayView { get; private set; }
        public Grid Grid { get; } = new Grid(BlockSize, BlockSize);
        public Plane Plane { get; } = new Plane(0);

        // Game state
        public GameState State { get; private set; }

        private Func<IEnumerable<TerrainSector>> SectorProbe;

        public PlatformerSceene(IStreamProvider streamProvider, IRenderer renderer, IUserInterface ui, ActionQueue actionQueue) 
            : this(streamProvider, renderer, ui, actionQueue, new TerrainFactory(TerrainDepth, TerrainHeight, TerrainSeaLevel))
        {
        }

        public PlatformerSceene(IStreamProvider streamProvider, IRenderer renderer, IUserInterface ui, ActionQueue actionQueue, ITerrainFactory terrainFactory)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.streamProvider = streamProvider;
            this.actionQueue = actionQueue;
            this.terrainFactory = terrainFactory;

            // Load latest game state
            var loaded = false;
            if (this.streamProvider.FileExists("state.json"))
            {
                loaded = LoadGame(this.streamProvider.ReadFile("state.json"));
            }

            if (!loaded)
            {
                NewGame();
            }
        }

        public void NewGame()
        {
            var state = GameState.Create(actionQueue, Grid, terrainFactory);
            ChangeGameState(state);
        }

        public bool LoadGame(Stream stream)
        {
            try
            {
                var state = GameState.LoadFromStream(actionQueue, Grid, terrainFactory, stream);
                ChangeGameState(state);
                return true;
            }
            catch
            {
            }

            return false;
        }

        private void ChangeGameState(GameState state)
        {
            State = state;

            SectorProbe = () => State.Terrain.Sectors;
            this.level = new ProceduralLevel(State.Terrain, Grid);
            var displaySize = Renderer.GetDisplaySize();
            DisplayView = new ViewportProjection(displaySize);
            DisplayView.Center(new Vector2(0, 0));

            ConfigureSingleScreen();
            //View.Scale(2.0f);
            //renderer.Scale(1, -1);
        }

        public override void Exiting()
        {
            State.SaveToStream(this.streamProvider.WriteFile("state.json"));
        }

        public override void Activate(InputMask uiInput, InputMask[] inputMasks)
        {
            base.Activate(uiInput, inputMasks);

            this.inputMasks = inputMasks;

            spriteResolver = new SpriteResolver(scope);
            spriteResolver.ResolveBindings(State.KnownPlayers.ToArray());

            debugFont = scope.LoadFont("ConsoleFont");

            var proceduralManager = new ProceduralObjectManager(State.Terrain, Grid, Plane);
            physics = new PhysicsEngine(proceduralManager, UpdateStep);

            level.Load(Plane);
            State.AddLevel(level.State);

            collectAction = new CollectAction();

            terrainWidget = new TerrainWidget(Renderer, State.Terrain, debugFont);
            dynamicGridWidget = new DynamicGridWidget(Renderer, debugFont, BlockSize);

            freeCameraController = new FreeCameraController(() => FirstCamera);
            playerController = new PlayerController(State, physics);

            playerController.PlayerActivated += UpdateCameraFocus;
            playerController.PlayerDeactivated += UpdateCameraFocus;

            TransitionToLevel(level.Name);
        }

        private void UpdateCameraFocus(object sender, PlayerGameStateEvent args)
        {
            //ConfigureSingleScreen();
            ConfigureSplitScreen();
        }

        private void ConfigureSingleScreen()
        {
            ActiveViews = new[] { new View(DisplayView, new Camera(DisplayView)) };
            var player = State.ActivePlayers.FirstOrDefault();
            if(player != null)
            {
                ActiveViews[0].Camera.Track(player);
                ActiveViews[0].Camera.Follow();
            }
        }

        private void ConfigureSplitScreen()
        {
            // Configure split screen
            ActiveViews = DisplayView.SplitVertically().Select(v => new View(v, new Camera(v))).ToArray();
            var player = State.ActivePlayers.FirstOrDefault();
            if(player != null)
            {
                ActiveViews[0].Camera.Track(player);
                ActiveViews[0].Camera.Follow();
                //ActiveViews[0].Viewport.Scale(2.0f);
                //ActiveViews[1].Camera.Track(State.ActivePlayers.Skip(1).First());
                ActiveViews[1].Camera.Track(player);
                ActiveViews[1].Camera.Follow();
                ActiveViews[1].Viewport.Scale(2.0f);
            }
        }

        public override void Update(FrameCounter counter, float timestep)
        {
            if (counter.HundredFrame)
            {
                JoinPlayers();
            }

            // Update level to generate terrain
            this.level.Update();

            // Camera follows player
            foreach(var view in ActiveViews)
            {
                view.Camera.Update();
            }

            // Handle UI inputs
            freeCameraController.Update(UiInput);

            foreach (var player in State.BoundPlayers)
            {
                playerController.Update(player);
            }

            // Apply physics to crate.
            var zeroVector = Vector2.Zero;
            foreach (var crate in State.ActiveLevel.Crates)
            {
                physics.ApplyToObject(crate, zeroVector);
            }

            // Apply physics to enemies.
            foreach (var enemy in State.ActiveLevel.Enemies)
            {
                enemy.Velocity = new Vector2(enemy.Facing.X * Constants.ENEMY_VELOCITY, 0);
                physics.ApplyToObject(enemy, zeroVector);
            }

            foreach (var projectile in State.ActiveLevel.Projectiles)
            {
                physics.ApplyToProjectile(projectile);
            }

            State.ActiveLevel.CleanUp();
        }

        private void JoinPlayers()
        {
            // Check for new players joining the game
            var unboundControls = inputMasks.Where(i => i.Bound == false);
            if (unboundControls.Any())
            {
                foreach(var newPlayer in State.BindPlayers(unboundControls))
                {
                    spriteResolver.ResolveBindings(newPlayer);
                    playerController.SpawnPlayer(newPlayer);
                }
                /*var playerToFollow = State.BoundPlayers.FirstOrDefault();
                if(playerToFollow != null)
                {
                    Camera.Track(playerToFollow);
                    Camera.Follow();
                }*/
            }
        }

        private void TransitionToLevel(string levelName)
        {
            State.SetActiveLevel(levelName);
        }

        public override void Prerender(FrameCounter counter, double gameTimeMsec)
        {
            Renderer.ResetTransform();
            foreach (var splitview in ActiveViews)
            {
                var viewport = splitview.Viewport;
                terrainWidget.Prerender(Grid, viewport, Plane);
            }
        }

        public override void Render(FrameCounter counter, double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);
            foreach (var splitview in ActiveViews)
            {
                var viewport = splitview.Viewport;
                Renderer.Begin();

                //Renderer.SetScissorRectangle(splitview.MapToViewport(Vector2.Zero), splitview.DisplaySize);
                Renderer.SetScissorRectangle(-viewport.Origin, viewport.DisplaySize);

                Renderer.ResetTransform();
                Renderer.Scale(1, -1);

                // Render terrain
                terrainWidget.Render(Grid, viewport, Plane);

                foreach (var obj in State.ActivePlayers.Where(p => !p.Dead))
                {
                    RenderObject(viewport, obj);
                }

                foreach (var obj in State.ActiveLevel.CollectableObjects)
                {
                    RenderRectangle(viewport, obj);
                }

                /*
                foreach (var obj in objectManager.RenderOrder)
                {
                    Renderer.RenderOpagueSprite(obj.SpriteBinding.Object, obj.Position, obj.Size, obj.Facing.X < 0);
                }

                foreach (var projectile in State.ActiveLevel.Projectiles)
                {
                    Renderer.RenderRectangle(projectile.Position, projectile.Size, projectile.Color);
                }
                */

                dynamicGridWidget.Render(viewport);

                Renderer.End();
            }
        }

        private void RenderObject(ViewportProjection view, IRenderableObject obj)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            var origin = new Vector2(obj.Position.X, obj.Position.Y + obj.Size.Y);
            var size = new Vector2(obj.Size.X, -obj.Size.Y);
            Renderer.RenderOpagueSprite(obj.SpriteBinding.Object, view.MapToViewport(origin), view.MapSizeToViewport(size), obj.Facing.X < 0);
        }

        private void RenderRectangle(ViewportProjection view, IRenderableObject obj)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            var origin = new Vector2(obj.Position.X, obj.Position.Y + obj.Size.Y);
            var size = new Vector2(obj.Size.X, -obj.Size.Y);
            Renderer.RenderRectangle(view.MapToViewport(origin), view.MapSizeToViewport(size), obj.Color);
        }

        public override string[] DiagnosticsString()
        {
            var lines = new List<string>();

            var sectors = SectorProbe().ToArray();
            lines.Add(string.Format("Views:"));
            foreach(var view in ActiveViews)
            {
                lines.Add(string.Format("{0}", view.Viewport.Projection.TopLeft));
            }
            lines.Add(string.Format("Sectors: {0}/{1}", sectors.Count(s => s.FullyLoaded), sectors.Count()));

            foreach(var player in State.ActivePlayers)
            {
                var itemName = player?.EquipedItem?.Name ?? "None";
                var itemQuantity = player?.EquipedItem?.Quantity ?? 0;
                lines.Add(
                    string.Format(
                        "{0}: {1}, {2} - {3} : {4}",
                        player?.PlayerBinding ?? "",
                        player?.Position ?? Vector2.Zero,
                        player?.Inventory?.TotalCount ?? 0,
                        itemName,
                        itemQuantity));
            }

            return lines.ToArray();
        }
    }
}