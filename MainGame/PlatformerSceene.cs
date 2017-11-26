using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Windows.Storage.Streams;
using Contracts;
using Prerelease.Main.Physics;
using VectorMath;
using Object = Prerelease.Main.Physics.Object;

namespace Prerelease.Main
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

        // Game state
        private GameState state;

        public PlatformerSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.actionQueue = actionQueue;
        }

        public override void Activate(InputMask[] inputMasks)
        {
            base.Activate(inputMasks);

            var levelFactory = new LevelFactory(actionQueue);
            var players = new List<PlayerObject>
            {
                new PlayerObject(actionQueue, inputMasks[0], new Vector2(), new Vector2(30, 30), Color.Red),
                new PlayerObject(actionQueue, inputMasks[1], new Vector2(), new Vector2(30, 30), Color.Green),
                new PlayerObject(actionQueue, inputMasks[2], new Vector2(), new Vector2(30, 30), Color.Blue),
                new PlayerObject(actionQueue, inputMasks[3], new Vector2(), new Vector2(30, 30), Color.Yellow),
            };

            spriteResolver = new SpriteResolver(scope);
            objectManager = new ObjectManager(players);
            state = new GameState(players);
            physics = new PhysicsEngine(objectManager, UpdateStep);

            var level1 = levelFactory.Load("Level1");
            var level2 = levelFactory.Load("Level2");
            spriteResolver.ResolveBindings(level1);
            spriteResolver.ResolveBindings(level2);
            state.AddLevel(level1);
            state.AddLevel(level2);
            TransitionToLevel(level1.Name);

            // Load content in active scope.
            foreach (var player in players)
            {
                player.SpriteBinding = LoadSprite("Chicken");
                player.SpriteBinding.Object.Size = new Vector2(30, 30);
            }
        }

        public override void Update(float timestep)
        {
            // Enter selected door now
            if (doorToEnter != null)
            {
                HandleTransition();
            }

            foreach (var player in state.Players)
            {
                HandlePlayerInput(player);
            }

            // Apply physics to crate.
            foreach (var crate in state.ActiveLevel.Crates)
            {
                physics.ApplyToObject(crate, ZeroVector);
            }

            // Apply physics to enemies.
            foreach (var enemy in state.ActiveLevel.Enemies)
            {
                enemy.Velocity.X = enemy.Facing.X*Constants.ENEMY_VELOCITY;
                physics.ApplyToObject(enemy, ZeroVector);
            }

            foreach (var projectile in state.ActiveLevel.Projectiles)
            {
                physics.ApplyToProjectile(projectile);
            }

            state.ActiveLevel.CleanUp();
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
            state.SetActiveLevel(levelName);
            objectManager.PartitionLevelObjects(state.ActiveLevel);

            // Spawn players at starting location
            foreach (var player in state.Players)
            {
                player.Acceleration.Clear();
                player.Velocity.Clear();
                player.Position = new Vector2(state.ActiveLevel.SpawnPoint);
            }
        }

        readonly Vector2 instantVelocity = Vector2.Zero;

        private void HandlePlayerInput(PlayerObject player)
        {
            var inputMask = player.InputMask;

            player.Active = inputMask.Input.Active;
            if (!player.Active)
                return;

            bool horizontalInput = false;

            instantVelocity.Clear();

            player.Acceleration.Clear();

            if (inputMask.Input.Restart)
            {
                player.Position.Clear();
                player.Velocity.Clear();
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
                    doorToEnter = state.ActiveLevel.Doors.FirstOrDefault(d => player.BoundingBox.Inside(d.Center));
                }
                if (inputMask.Input.Up)
                {
                    instantVelocity.Y = Constants.JUMP_SPEED;
                }
            }

            var horizontalControl = player.Grounded ? Constants.GROUND_ACCELERATION : Constants.AIR_ACCELERATION;
            if (inputMask.Input.Left)
            {
                horizontalInput = true;
                player.Acceleration.X -= horizontalControl;
                player.Facing.X = -1;
            }
            if (inputMask.Input.Right)
            {
                horizontalInput = true;
                player.Acceleration.X += horizontalControl;
                player.Facing.X = 1;
            }

            // If player is on the ground and not moving, come to a complete horizontal stop to prevent drift
            if (player.Grounded && !horizontalInput)
            {
                player.Acceleration.X = 0.0f;
                player.Velocity.X = 0.0f;
            }

            if (inputMask.Input.Fire && player.Weapon.CanFire)
            {
                state.ActiveLevel.AddProjectiles(FireWeapon(player));
            }

            player.Weapon.Update();

            inputMask.Reset();

            physics.ApplyToObject(player, instantVelocity);
        }

        private Projectile FireWeapon(PlayerObject player)
        {
            player.Weapon.Cooldown = 10;
            return new Projectile(actionQueue, player, player.Center, new Vector2(Constants.PROJECTILE_VELOCITY * player.Facing.X, 0.0f), new Vector2(1,1));
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);

            foreach (var obj in objectManager.RenderOrder)
            {
                Renderer.RenderOpagueSprite(obj.SpriteBinding.Object, obj.Position, obj.Size, obj.Facing.X < 0);
            }

            foreach (var projectile in state.ActiveLevel.Projectiles)
            {
                Renderer.RenderRectangle(projectile.Position, projectile.Size, Color.Red);
            }
        }
    }
}