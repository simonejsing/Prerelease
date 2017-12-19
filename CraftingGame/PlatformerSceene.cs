using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using CraftingGame.Physics;
using Terrain;
using VectorMath;

namespace CraftingGame
{
    public class PlatformerSceene : Sceene
    {
        public const int BlockSize = 30;

        // Use a fixed update step size.
        private const float UpdateStep = 1.0f;
        private readonly Vector2 ZeroVector = Vector2.Zero;

        private readonly ActionQueue actionQueue;
        private SpriteResolver spriteResolver;
        private PhysicsEngine physics;
        private ObjectManager objectManager;
        private Door doorToEnter = null;
        private IFont debugFont = null;

        public ViewportProjection View { get; }
        private Rect2 ActiveView => View.Projection;

        // Terrain
        private int plane = 0;
        private readonly Grid grid = new Grid(BlockSize, BlockSize);
        private readonly CachedTerrainGenerator terrainGenerator;

        public Grid Grid => grid;

        // Game state
        public GameState State { get; private set; }

        public PlatformerSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue, ITerrainGenerator terrain = null)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.terrainGenerator = new CachedTerrainGenerator(
                terrain ?? new Generator(100, 100, 80));
            //this.terrainGenerator = terrain ?? new Generator(100, 100, 80);
            var viewPort = renderer.GetViewport();
            View = new ViewportProjection(viewPort);
            View.Center(new Vector2(0, 0));
            View.Scale(2.0f);
            renderer.Scale(1, -1);
            this.actionQueue = actionQueue;
        }

        public override void Activate(InputMask uiInput, InputMask[] inputMasks)
        {
            base.Activate(uiInput, inputMasks);

            var levelFactory = new LevelFactory(actionQueue);
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

            objectManager = new ObjectManager(players);
            State = new GameState(players);

            var proceduralManager = new ProceduralObjectManager(terrainGenerator, grid, new Plane(plane));
            //physics = new PhysicsEngine(objectManager, UpdateStep);
            physics = new PhysicsEngine(proceduralManager, UpdateStep);

            var level1 = levelFactory.Load("Level1");
            var level2 = levelFactory.Load("Level2");
            spriteResolver.ResolveBindings(level1);
            spriteResolver.ResolveBindings(level2);
            State.AddLevel(level1);
            State.AddLevel(level2);
            TransitionToLevel(level1.Name);

            TransitionToProceduralLevel();

            terrainGenerator.SetActiveSector((int)ActiveView.TopLeft.X, (int)ActiveView.TopLeft.Y, plane);
        }

        private void TransitionToProceduralLevel()
        {
            // Initialize the starting sector(s) based on the active view
            var points = new[] { ActiveView.TopLeft, ActiveView.BottomLeft, ActiveView.TopRight, ActiveView.BottomRight };
            foreach(var point in points)
            {
                var coord = grid.PointToGridCoordinate(point);
                terrainGenerator.Generate(coord, new Plane(plane));
            }
        }

        public override string[] DiagnosticsString()
        {
            var player = State.Players.FirstOrDefault();
            var playerPos = player?.Position ?? Vector2.Zero;
            return new[]
            {
                string.Format("View: {0}", ActiveView.TopLeft),
                string.Format("Sectors: {0}/{1}", terrainGenerator.Sectors.Count(s => s.FullyLoaded), terrainGenerator.Sectors.Count()),
                string.Format("Player: {0}", playerPos),
            };
        }

        public void GenerateTerrainSectors()
        {
            terrainGenerator.Update(-1);
        }

        public override void Update(float timestep)
        {
            // Enter selected door now
            if (doorToEnter != null)
            {
                HandleTransition();
            }

            terrainGenerator.Update(200);

            // Handle UI inputs
            HandleUiInput();

            foreach (var player in State.Players)
            {
                HandlePlayerInput(player);
            }

            // Apply physics to crate.
            foreach (var crate in State.ActiveLevel.Crates)
            {
                physics.ApplyToObject(crate, ZeroVector);
            }

            // Apply physics to enemies.
            foreach (var enemy in State.ActiveLevel.Enemies)
            {
                enemy.Velocity = new Vector2(enemy.Facing.X*Constants.ENEMY_VELOCITY, 0);
                physics.ApplyToObject(enemy, ZeroVector);
            }

            foreach (var projectile in State.ActiveLevel.Projectiles)
            {
                physics.ApplyToProjectile(projectile);
            }

            State.ActiveLevel.CleanUp();
        }

        private void HandleUiInput()
        {
            const int ScrollSpeed = 50;
            var translation = new Vector2();
            if (UiInput.Input.Up)
                translation += new Vector2(0, ScrollSpeed);
            else if (UiInput.Input.Down)
                translation += new Vector2(0, -ScrollSpeed);
            if (UiInput.Input.Left)
                translation += new Vector2(-ScrollSpeed, 0);
            else if (UiInput.Input.Right)
                translation += new Vector2(ScrollSpeed, 0);

            View.Translate(translation);
        }

        private void HandleTransition()
        {
            switch (doorToEnter.Destination.Type)
            {
                case DestinationType.Level:
                    TransitionToLevel(doorToEnter.Destination.Identifier);
                    break;
            }
        }

        private void TransitionToLevel(string levelName)
        {
            State.SetActiveLevel(levelName);
            objectManager.PartitionLevelObjects(State.ActiveLevel);

            // Spawn players at starting location
            foreach (var player in State.Players)
            {
                player.Acceleration = Vector2.Zero;
                player.Velocity = Vector2.Zero;
                player.Position = new Vector2(State.ActiveLevel.SpawnPoint);
            }
        }

        Vector2 instantVelocity = Vector2.Zero;

        private void HandlePlayerInput(PlayerObject player)
        {
            var inputMask = player.InputMask;

            player.Active = inputMask.Input.Active;
            if (!player.Active)
                return;

            bool horizontalInput = false;

            instantVelocity = Vector2.Zero;

            player.Acceleration = Vector2.Zero;

            if (inputMask.Input.Restart)
            {
                player.Position = Vector2.Zero;
                player.Velocity = Vector2.Zero;
                player.Weapon.Cooldown = 0;
                player.HitPoints = 1;
            }

            if (player.Dead)
                return;

            if (player.Grounded)
            {
                if (inputMask.Input.Select)
                {
                    // Enter door if on door sprite (next frame update will carry out the transition
                    doorToEnter = State.ActiveLevel.Doors.FirstOrDefault(d => player.BoundingBox.Intersects(d.Center));
                }
                if (inputMask.Input.Up)
                {
                    instantVelocity = new Vector2(0, Constants.JUMP_SPEED);
                }
            }

            var horizontalControl = player.Grounded ? Constants.GROUND_ACCELERATION : Constants.AIR_ACCELERATION;
            if (inputMask.Input.Left)
            {
                horizontalInput = true;
                player.Acceleration -= new Vector2(horizontalControl, 0);
                player.Facing = new Vector2(-1, 0);
            }
            if (inputMask.Input.Right)
            {
                horizontalInput = true;
                player.Acceleration += new Vector2(horizontalControl, 0);
                player.Facing = new Vector2(1, 0);
            }

            // If player is on the ground and not moving, come to a complete horizontal stop to prevent drift
            if (player.Grounded && !horizontalInput)
            {
                player.Acceleration = new Vector2(0.0f, player.Acceleration.Y);
                player.Velocity = new Vector2(0.0f, player.Velocity.Y);
            }

            if (inputMask.Input.Fire && player.Weapon.CanFire)
            {
                State.ActiveLevel.AddProjectiles(FireWeapon(player));
            }

            player.Weapon.Update();

            inputMask.Reset();

            physics.ApplyToObject(player, instantVelocity);

            // Check if player is touching any collectable objects
            // TODO: Quad-tree to partition space such that we don't have to test intersection with every single collectable object on each frame.
            foreach (var touchedObject in State.ActiveLevel.CollectableObjects.Where(o => o.BoundingBox.Intersects(player.BoundingBox)))
            {
                touchedObject.OnCollect(player);
                touchedObject.PickedUp = true;
            }
        }

        private Projectile FireWeapon(PlayerObject player)
        {
            player.Weapon.Cooldown = 10;
            return new Projectile(actionQueue, player, player.Color, player.Center, new Vector2(Constants.PROJECTILE_VELOCITY * player.Facing.X, 0.0f), new Vector2(1,1));
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);

            // Render terrain
            var startCoord = grid.PointToGridCoordinate(ActiveView.BottomLeft);
            var numberCells = grid.PointToGridCoordinate(ActiveView.Size);

            for (var v = 0; v <= numberCells.V; v++)
            {
                for (var u = 0; u <= numberCells.U; u++)
                {
                    var coord = startCoord + new Coordinate(u, v);
                    var block = terrainGenerator[coord, new Plane(plane)];
                    var position = grid.GridCoordinateToPoint(coord);
                    switch (block.Type)
                    {
                        case TerrainType.Dirt:
                            RenderRectangle(position, grid.Size, Color.Yellow);
                            break;
                        case TerrainType.Rock:
                            RenderRectangle(position, grid.Size, Color.Gray);
                            break;
                        case TerrainType.Bedrock:
                            RenderRectangle(position, grid.Size, Color.DarkGray);
                            break;
                        case TerrainType.Sea:
                            RenderRectangle(position, grid.Size, Color.Blue);
                            break;
                        case TerrainType.Free:
                            break;
                    }
                }
            }

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

            RenderDynamicGrid();
        }

        private void RenderRectangle(Vector2 point, Vector2 size, Color color)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            point = new Vector2(point.X, point.Y + size.Y);
            size = new Vector2(size.X, -size.Y);
            Renderer.RenderRectangle(View.MapToViewport(point), View.MapSizeToViewport(size), color);
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

        private void RenderDynamicGrid()
        {
            var gridSize = BlockSize*10;

            var gridTop = (int)Math.Floor(ActiveView.TopLeft.Y / gridSize);
            var gridBottom = (int)Math.Floor(ActiveView.BottomLeft.Y / gridSize);

            //var gridBottom = (int)Math.Floor(View.MapToWorld(Vector2.Zero).Y / gridSize);
            //var gridTop = (int)Math.Floor(View.MapSizeToWorld(View.ViewPort).Y / gridSize);

            for (var y = gridBottom; y <= gridTop; y += 1)
            {
                var c = y >= 0 ? Color.Blue : Color.Red;
                if (y == 0)
                    c = Color.DarkGray;
                var p = View.MapToViewport(new Vector2(0, y * gridSize));
                Renderer.RenderVector(new Vector2(0, p.Y), new Vector2(View.ViewPort.X, 0), c, 3);

                // Render y-labels
                Renderer.RenderText(
                    debugFont,
                    View.MapToViewport(new Vector2(0, y * gridSize)),
                    $"(0,{y * gridSize})",
                    c);
            }

            var gridLeft = (int)Math.Floor(ActiveView.TopLeft.X / gridSize);
            var gridRight = (int)Math.Floor(ActiveView.TopRight.X / gridSize);
            for (var x = gridLeft; x <= gridRight; x += 1)
            {
                var c = x >= 0 ? Color.Blue : Color.Red;
                if (x == 0)
                    c = Color.DarkGray;
                var p = View.MapToViewport(new Vector2(x * gridSize, 0));
                Renderer.RenderVector(new Vector2(p.X, 0), new Vector2(0, -View.ViewPort.Y), c, 3);

                // Render x-labels
                Renderer.RenderText(
                    debugFont,
                    View.MapToViewport(new Vector2(x * gridSize, 0)),
                    $"({x * gridSize},0)",
                    c);
            }
        }
    }
}