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
        private readonly CachedTerrainGenerator cachedTerrain;
        private readonly ProceduralLevel level;

        // Widgets
        private TerrainWidget terrainWidget;
        private DynamicGridWidget dynamicGridWidget;

        // Actions
        private CollectAction collectAction;
        private DigAction digAction;

        // Input controllers
        private FreeCameraController freeCameraController;
        private PlayerController playerController;

        public IModifiableTerrain Terrain => level.Terrain;
        public Camera Camera { get; }
        public ViewportProjection View { get; }
        public Grid Grid { get; } = new Grid(BlockSize, BlockSize);
        public Plane Plane { get; } = new Plane(0);

        // Game state
        public GameState State { get; }

        public PlatformerSceene(IStreamProvider streamProvider, IRenderer renderer, IUserInterface ui, ActionQueue actionQueue, ITerrainGenerator terrain = null)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.streamProvider = streamProvider;
            State = new GameState(actionQueue);

            // Load latest game state
            if (this.streamProvider.FileExists("state.json"))
            {
                try
                {
                    State.LoadFromStream(this.streamProvider.ReadFile("state.json"));
                }
                catch
                {
                }
            }

            this.cachedTerrain = new CachedTerrainGenerator(
                terrain ?? new Generator(TerrainDepth, TerrainHeight, TerrainSeaLevel));
            this.level = new ProceduralLevel(this.cachedTerrain, Grid, Plane);
            var viewPort = renderer.GetViewport();
            View = new ViewportProjection(viewPort);
            View.Center(new Vector2(0, 0));
            this.Camera = new Camera(View);
            //View.Scale(2.0f);
            renderer.Scale(1, -1);
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
            spriteResolver.ResolveBindings(State.Players.ToArray());

            debugFont = scope.LoadFont("ConsoleFont");

            var proceduralManager = new ProceduralObjectManager(cachedTerrain, Grid, Plane);
            physics = new PhysicsEngine(proceduralManager, UpdateStep);

            level.Load(View);
            State.AddLevel(level.State);

            var activeView = View.Projection;
            cachedTerrain.SetActiveSector((int)activeView.TopLeft.X, (int)activeView.TopLeft.Y, Plane.W);

            collectAction = new CollectAction();
            digAction = new DigAction(ActionQueue, collectAction, State, Grid, cachedTerrain);

            terrainWidget = new TerrainWidget(Renderer, Terrain);
            dynamicGridWidget = new DynamicGridWidget(Renderer, debugFont, BlockSize);

            freeCameraController = new FreeCameraController(Camera);
            playerController = new PlayerController(State, physics, Camera);
            playerController.Dig += digAction.Invoke;

            TransitionToLevel(level.Name);
        }

        public override void Update(float timestep)
        {
            // TODO: This does not need to happen every frame!
            JoinPlayers();

            cachedTerrain.Update(200);

            // Camera follows player
            Camera.Update();

            // Handle UI inputs
            freeCameraController.Update(UiInput);

            // TODO: This will be inefficient if lots of players have joined the game and the state keeps track of them all
            foreach (var player in State.Players.Where(p => p.InputBound))
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
            foreach (var inputMask in inputMasks)
            {
                // Find matching player
                var player = State.Players.FirstOrDefault(p => p.PlayerBinding == inputMask.PlayerBinding);
                if (player == null)
                {
                    // New player joined
                    CreatePlayer(inputMask);
                }
                else if (!player.InputBound)
                {
                    // Reconnect
                    player.BindInput(inputMask);
                }
            }
        }

        private void CreatePlayer(InputMask inputMask)
        {
            var player = new PlayerObject(ActionQueue, Guid.NewGuid(), inputMask.PlayerBinding, new Plane(0), Vector2.Zero, new Vector2(30, 30), "Chicken", Color.Red);
            spriteResolver.ResolveBindings(player);
            playerController.SpawnPlayer(player);
            player.BindInput(inputMask);
            State.AddPlayer(player);
        }

        private void TransitionToLevel(string levelName)
        {
            State.SetActiveLevel(levelName);
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);

            // Render terrain
            terrainWidget.Render(Grid, View, Plane);

            foreach (var obj in State.ActivePlayers.Where(p => !p.Dead))
            {
                RenderObject(obj);
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

        public override string[] DiagnosticsString()
        {
            var player = State.Players.FirstOrDefault();
            var playerPos = player?.Position ?? Vector2.Zero;
            return new[]
            {
                string.Format("View: {0}", View.Projection.TopLeft),
                string.Format("Sectors: {0}/{1}", cachedTerrain.Sectors.Count(s => s.FullyLoaded), cachedTerrain.Sectors.Count()),
                string.Format("Player: {0}", playerPos),
                string.Format("Inventory: {0}", player?.Inventory?.TotalCount ?? 0),
            };
        }
    }
}