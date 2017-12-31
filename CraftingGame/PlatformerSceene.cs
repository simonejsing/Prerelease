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
using CraftingGame.Items;

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
        private SpriteResolver spriteResolver;
        private PhysicsEngine physics;
        private IFont debugFont = null;
        private InputMask[] inputMasks;

        // Terrain
        private readonly ProceduralLevel level;

        // Widgets
        private TerrainWidget terrainWidget;
        private DynamicGridWidget dynamicGridWidget;

        // Actions
        private CollectAction collectAction;

        // Input controllers
        private FreeCameraController freeCameraController;
        private PlayerController playerController;

        public IModifiableTerrain Terrain => State.Terrain;
        public Camera Camera { get; }
        public ViewportProjection View { get; }
        public Grid Grid { get; } = new Grid(BlockSize, BlockSize);
        public Plane Plane { get; } = new Plane(0);

        // Game state
        public GameState State { get; }

        private readonly Func<IEnumerable<TerrainSector>> SectorProbe;

        public PlatformerSceene(IStreamProvider streamProvider, IRenderer renderer, IUserInterface ui, ActionQueue actionQueue) 
            : this(streamProvider, renderer, ui, actionQueue, new TerrainFactory(TerrainDepth, TerrainHeight, TerrainSeaLevel))
        {
        }

        public PlatformerSceene(IStreamProvider streamProvider, IRenderer renderer, IUserInterface ui, ActionQueue actionQueue, ITerrainFactory terrainFactory)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.streamProvider = streamProvider;
            State = new GameState(actionQueue, Grid, terrainFactory);

            // Load latest game state
            var loaded = false;
            if (this.streamProvider.FileExists("state.json"))
            {
                try
                {
                    State.LoadFromStream(this.streamProvider.ReadFile("state.json"));
                    loaded = true;
                }
                catch
                {
                }
            }

            if(!loaded)
            {
                // Start new game
                State.Terrain = new CachedTerrainGenerator(terrainFactory.Create());
            }

            SectorProbe = () => State.Terrain.Sectors;
            this.level = new ProceduralLevel(State.Terrain, Grid);
            var viewPort = renderer.GetViewport();
            View = new ViewportProjection(viewPort);
            View.Center(new Vector2(0, 0));
            this.Camera = new Camera(View);
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

            level.Load(View, Plane);
            State.AddLevel(level.State);

            var activeView = View.Projection;
            State.Terrain.SetActiveSector((int)activeView.TopLeft.X, (int)activeView.TopLeft.Y, Plane.W);

            collectAction = new CollectAction();

            terrainWidget = new TerrainWidget(Renderer, State.Terrain, debugFont);
            dynamicGridWidget = new DynamicGridWidget(Renderer, debugFont, BlockSize);

            freeCameraController = new FreeCameraController(Camera);
            playerController = new PlayerController(State, physics, Camera);

            TransitionToLevel(level.Name);
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
            Camera.Update();

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

                var playerToFollow = State.BoundPlayers.FirstOrDefault();
                if(playerToFollow != null)
                {
                    Camera.Track(playerToFollow);
                    Camera.Follow();
                }
            }
        }

        private void TransitionToLevel(string levelName)
        {
            State.SetActiveLevel(levelName);
        }

        public override void Prerender(FrameCounter counter, double gameTimeMsec)
        {
            Renderer.ResetTransform();
            terrainWidget.Prerender(Grid, View, Plane);
        }

        public override void Render(FrameCounter counter, double gameTimeMsec)
        {
            Renderer.ResetTransform();
            Renderer.Scale(1, -1);
            Renderer.Clear(Color.Black);

            // Render terrain
            terrainWidget.Render(Grid, View, Plane);

            foreach (var obj in State.ActivePlayers.Where(p => !p.Dead))
            {
                RenderObject(obj);
            }

            foreach(var obj in State.ActiveLevel.CollectableObjects)
            {
                RenderRectangle(obj);
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

            //dynamicGridWidget.Render(View);
        }

        private void RenderObject(IRenderableObject obj)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            var origin = new Vector2(obj.Position.X, obj.Position.Y + obj.Size.Y);
            var size = new Vector2(obj.Size.X, -obj.Size.Y);
            Renderer.RenderOpagueSprite(obj.SpriteBinding.Object, View.MapToViewport(origin), View.MapSizeToViewport(size), obj.Facing.X < 0);
        }

        private void RenderRectangle(IRenderableObject obj)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            var origin = new Vector2(obj.Position.X, obj.Position.Y + obj.Size.Y);
            var size = new Vector2(obj.Size.X, -obj.Size.Y);
            Renderer.RenderRectangle(View.MapToViewport(origin), View.MapSizeToViewport(size), obj.Color);
        }

        public override string[] DiagnosticsString()
        {
            var lines = new List<string>();

            var sectors = SectorProbe().ToArray();
            lines.Add(string.Format("View: {0}", View.Projection.TopLeft));
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