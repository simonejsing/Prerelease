using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using CraftingGame.Controllers;
using CraftingGame.Physics;
using CraftingGame.Widgets;
using Terrain;
using VectorMath;
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

        // Input controllers
        private FreeCameraController freeCameraController;
        private PlayerController playerController;

        public ITerrainGenerator Terrain => level.Terrain;
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
                new PlayerObject(actionQueue, inputMasks[0], new Vector2(), new Vector2(30, 30), "Chicken", Color.Red),
                new PlayerObject(actionQueue, inputMasks[1], new Vector2(), new Vector2(30, 30), "Chicken", Color.Green),
                new PlayerObject(actionQueue, inputMasks[2], new Vector2(), new Vector2(30, 30), "Chicken", Color.Blue),
                new PlayerObject(actionQueue, inputMasks[3], new Vector2(), new Vector2(30, 30), "Chicken", Color.Yellow),
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

            terrainWidget = new TerrainWidget(Renderer, Terrain);
            dynamicGridWidget = new DynamicGridWidget(Renderer, debugFont, BlockSize);

            freeCameraController = new FreeCameraController(Camera);
            playerController = new PlayerController(State, physics, Camera);
            playerController.Dig += OnPlayerDig;

            TransitionToLevel(level.Name);
        }

        private void OnPlayerDig(object sender, PlayerObject player)
        {
            // Find the spot below and in front of the player's center
            var playerCoord = Grid.PointToGridCoordinate(player.Center);
            var digCoord = new Coordinate(playerCoord.U + Math.Sign(player.Facing.X), playerCoord.V);

            // Can the player dig here?
            cachedTerrain.Generate(digCoord, Plane);
            var type = cachedTerrain[digCoord, Plane].Type;
            if (type == TerrainType.Free)
            {
                // No, try below
                digCoord = new Coordinate(digCoord.U, digCoord.V - 1);
                cachedTerrain.Generate(digCoord, Plane);
                type = cachedTerrain[digCoord, Plane].Type;
            }

            // Dig it!
            switch (type)
            {
                case TerrainType.Dirt:
                case TerrainType.Rock:
                    // Yes! Then get to work...
                    cachedTerrain.Destroy(digCoord, Plane);

                    // Drop an item of the terrain type.
                    var item = ItemFactory.FromTerrain(type);
                    if(item != null)
                    {
                        var size = new Vector2(10, 10);
                        var coordPos = Grid.GridCoordinateToPoint(digCoord);
                        var position = coordPos + 0.5f * (Grid.Size - size);
                        var itemObject = new ItemObject(ActionQueue, position, size, item);
                        itemObject.Collect += PickUpItem;
                        State.ActiveLevel.AddCollectableObjects(itemObject);
                    }
                    break;
            }
        }

        private void PickUpItem(object sender, ICollectableObject source, ICollectingObject target)
        {
            target.Inventory.Add(source.Item.Name);
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