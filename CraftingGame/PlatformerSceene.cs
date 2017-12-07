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
        // Use a fixed update step size.
        private const float UpdateStep = 1.0f;
        private readonly Vector2 ZeroVector = Vector2.Zero;

        private readonly ActionQueue actionQueue;
        private SpriteResolver spriteResolver;
        private PhysicsEngine physics;
        private ObjectManager objectManager;
        private Door doorToEnter = null;

        public Rect2 ActiveView { get; set; }

        // Terrain
        private readonly Vector2 GridSize = new Vector2(30, 30);
        private readonly CachedTerrainGenerator terrainGenerator;

        // Game state
        public GameState State { get; private set; }

        public PlatformerSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue, ITerrainGenerator terrain = null)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.terrainGenerator = new CachedTerrainGenerator(
                terrain ?? new Generator(100, 100, 0));
            ActiveView = new Rect2(new Vector2(0, 310), renderer.GetViewport());
            this.actionQueue = actionQueue;
        }

        public override void Activate(InputMask uiInput, InputMask[] inputMasks)
        {
            base.Activate(uiInput, inputMasks);

            var levelFactory = new LevelFactory(actionQueue);
            spriteResolver = new SpriteResolver(scope);

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
            physics = new PhysicsEngine(objectManager, UpdateStep);

            var level1 = levelFactory.Load("Level1");
            var level2 = levelFactory.Load("Level2");
            spriteResolver.ResolveBindings(level1);
            spriteResolver.ResolveBindings(level2);
            State.AddLevel(level1);
            State.AddLevel(level2);
            TransitionToLevel(level1.Name);

            terrainGenerator.SetActiveSector((int)ActiveView.TopLeft.X, (int)ActiveView.TopLeft.Y, 0);
        }

        public override void Update(float timestep)
        {
            // Enter selected door now
            if (doorToEnter != null)
            {
                HandleTransition();
            }

            terrainGenerator.Update(-1);

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
            ActiveView.TopLeft += new Vector2(0, UiInput.Input.Up ? ScrollSpeed : 0);
            ActiveView.TopLeft += new Vector2(0, UiInput.Input.Down ? -ScrollSpeed : 0);
            ActiveView.TopLeft += new Vector2(UiInput.Input.Left ? -ScrollSpeed : 0, 0);
            ActiveView.TopLeft += new Vector2(UiInput.Input.Right ? ScrollSpeed : 0, 0);
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
            var sx = (int)Math.Ceiling(ActiveView.TopLeft.X / GridSize.X);
            var sy = (int)Math.Ceiling(ActiveView.TopLeft.Y / GridSize.Y);
            var ox = (int)Math.Round(sx * GridSize.X - ActiveView.TopLeft.X);
            var oy = (int)Math.Round(sy * GridSize.Y - ActiveView.TopLeft.Y);
            var nx = (int)Math.Ceiling(ActiveView.Size.X / GridSize.X);
            var ny = (int)Math.Ceiling(ActiveView.Size.Y / GridSize.Y);

            var plane = 0;
            // view port has negative height
            for (var y = ny; y <= 0; y++)
            {
                for (var x = -1; x < nx; x++)
                {
                    var block = terrainGenerator[sx + x, sy + y, plane];
                    var p = new Vector2(ox + x * GridSize.X, -(oy + y * GridSize.Y));
                    switch (block.Type)
                    {
                        case TerrainType.Dirt:
                            Renderer.RenderRectangle(p, GridSize, Color.LightGray);
                            break;
                        case TerrainType.Rock:
                            Renderer.RenderRectangle(p, GridSize, Color.Gray);
                            break;
                        case TerrainType.Bedrock:
                            Renderer.RenderRectangle(p, GridSize, Color.DarkGray);
                            break;
                        case TerrainType.Free:
                            //Renderer.RenderRectangle(p, GridSize, Color.Blue);
                            break;
                    }
                }
            }

            /*
            foreach (var obj in objectManager.RenderOrder)
            {
                Renderer.RenderOpagueSprite(obj.SpriteBinding.Object, obj.Position, obj.Size, obj.Facing.X < 0);
            }

            foreach (var projectile in state.ActiveLevel.Projectiles)
            {
                Renderer.RenderRectangle(projectile.Position, projectile.Size, projectile.Color);
            }
            */
        }
    }
}