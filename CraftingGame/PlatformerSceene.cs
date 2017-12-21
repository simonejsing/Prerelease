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

        private readonly ActionQueue actionQueue;
        private SpriteResolver spriteResolver;
        private PhysicsEngine physics;
        private IFont debugFont = null;

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
        public GameState State { get; private set; }

        public PlatformerSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue, ITerrainGenerator terrain = null)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.cachedTerrain = new CachedTerrainGenerator(
                terrain ?? new Generator(TerrainDepth, TerrainHeight, TerrainSeaLevel));
            this.level = new ProceduralLevel(this.cachedTerrain, Grid, Plane);
            var viewPort = renderer.GetViewport();
            View = new ViewportProjection(viewPort);
            View.Center(new Vector2(0, 0));
            this.Camera = new Camera(View);
            //View.Scale(2.0f);
            renderer.Scale(1, -1);
            this.actionQueue = actionQueue;
        }

        public override void Activate(InputMask uiInput, InputMask[] inputMasks)
        {
            base.Activate(uiInput, inputMasks);

            spriteResolver = new SpriteResolver(scope);
            debugFont = scope.LoadFont("ConsoleFont");

            var players = new List<PlayerObject>
            {
                new PlayerObject(actionQueue, inputMasks[0], new Plane(0), new Vector2(), new Vector2(30, 30), "Chicken", Color.Red),
                new PlayerObject(actionQueue, inputMasks[1], new Plane(0), new Vector2(), new Vector2(30, 30), "Chicken", Color.Green),
                new PlayerObject(actionQueue, inputMasks[2], new Plane(0), new Vector2(), new Vector2(30, 30), "Chicken", Color.Blue),
                new PlayerObject(actionQueue, inputMasks[3], new Plane(0), new Vector2(), new Vector2(30, 30), "Chicken", Color.Yellow),
            };
            spriteResolver.ResolveBindings(players);

            State = new GameState(players);
            Camera.Track(State.Players.First());
            Camera.Follow();

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
            // Enter selected door now
            if (State.DoorToEnter != null)
            {
                TransitionThroughDoor(State.DoorToEnter);
            }

            cachedTerrain.Update(200);

            // Camera follows player
            Camera.Update();

            // Handle UI inputs
            freeCameraController.Update(UiInput);

            foreach (var player in State.Players)
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
                enemy.Velocity = new Vector2(enemy.Facing.X*Constants.ENEMY_VELOCITY, 0);
                physics.ApplyToObject(enemy, zeroVector);
            }

            foreach (var projectile in State.ActiveLevel.Projectiles)
            {
                physics.ApplyToProjectile(projectile);
            }

            State.ActiveLevel.CleanUp();
        }

        private void TransitionThroughDoor(Door door)
        {
            switch (door.Destination.Type)
            {
                case DestinationType.Level:
                    TransitionToLevel(door.Destination.Identifier);
                    break;
            }
        }

        private void TransitionToLevel(string levelName)
        {
            State.SetActiveLevel(levelName);

            // Spawn players at starting location
            foreach (var player in State.Players)
            {
                playerController.SpawnPlayer(player);
            }
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);

            // Render terrain
            terrainWidget.Render(Grid, View, Plane);

            foreach (var obj in State.Players.Where(p => p.Active && !p.Dead))
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