using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Prerelease.Main.Physics;
using VectorMath;

namespace Prerelease.Main
{
    public class PlatformerSceene : Sceene
    {
        // Use a fixed update step size.
        private const float UpdateStep = 1.0f;
        private readonly Vector2 ZeroVector = Vector2.Zero;

        private readonly ActionQueue actionQueue;
        private readonly PlayerObject[] players;
        private readonly BlockGrid blocks;
        private readonly List<Projectile> projectiles = new List<Projectile>();
        private readonly MovableObject crate, crate2;
        private readonly PhysicsEngine physics;
        private ISprite blockSprite;

        public PlatformerSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue)
            : base("Platformer", renderer, ui, actionQueue)
        {
            this.actionQueue = actionQueue;
            players = new[]
            {
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Red),
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Green),
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Blue),
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Yellow),
            };

            //crate = new MovableObject(actionQueue, new Vector2(30 * 22, 0), new Vector2(30, 30));
            crate = new MovableObject(actionQueue, new Vector2(30 * 5, 30), new Vector2(30, 30));
            crate2 = new MovableObject(actionQueue, new Vector2(30 * 2.5f, 270 - 45), new Vector2(30, 30));

            blocks = new BlockGrid(30, 30, 100, 100);
            physics = new PhysicsEngine(blocks, UpdateStep);
            physics.AddCollidableObject(crate);
            physics.AddCollidableObject(crate2);

            for (uint i = 0; i < 40; i++)
            {
                blocks.Insert(10, i);
                if (i > 15)
                {
                    blocks.Insert(5, i);
                }
            }

            blocks.Insert(2, 20);
            blocks.Insert(3, 20);
            blocks.Insert(4, 20);

            blocks.Insert(9, 8);
            blocks.Insert(9, 39);

            blocks.Insert(8, 10);
            blocks.Insert(7, 11);
            blocks.Insert(6, 12);
            blocks.Insert(5, 13);
            blocks.Insert(4, 14);
            blocks.Insert(3, 15);
        }

        public override void Activate()
        {
            base.Activate();

            // Load content in active scope.
            blockSprite = LoadSprite("Block");
            foreach (var player in players)
            {
                player.Sprite = LoadSprite("Chicken");
                player.Sprite.Size = new Vector2(30, 30);
            }
        }

        public override void Update(float timestep, InputMask[] inputMasks)
        {
            for (var i = 0; i < players.Length; i++)
            {
                HandlePlayerInput(players[i], inputMasks[i]);
            }

            // Apply physics to crate.
            physics.Apply(crate, ZeroVector);

            foreach (var projectile in projectiles)
            {
                projectile.Update(UpdateStep);
            }

            // Delete expired projectiles
            projectiles.RemoveAll(p => p.Expired);
        }

        readonly Vector2 instantVelocity = Vector2.Zero;

        private void HandlePlayerInput(PlayerObject player, InputMask inputMask)
        {
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
            }

            if (player.CanAccelerate)
            {
                if (inputMask.Input.Up)
                {
                    instantVelocity.Y = Constants.JUMP_SPEED;
                }
            }

            var horizontalControl = player.CanAccelerate ? Constants.GROUND_ACCELERATION : Constants.AIR_ACCELERATION;
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
            if (player.CanAccelerate && !horizontalInput)
            {
                player.Acceleration.X = 0.0f;
                player.Velocity.X = 0.0f;
            }

            if (inputMask.Input.Select)
            {
            }

            if (inputMask.Input.Fire && player.Weapon.CanFire)
            {
                projectiles.Add(FireWeapon(player));
            }

            player.Weapon.Update();

            inputMask.Reset();

            physics.Apply(player, instantVelocity);
        }

        private Projectile FireWeapon(PlayerObject player)
        {
            player.Weapon.Cooldown = 10;
            return new Projectile(actionQueue, player, player.Center, new Vector2(Constants.PROJECTILE_VELOCITY * player.Facing.X, 0.0f), new Vector2(1,1));
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);

            foreach (var block in blocks.OccupiedBlocks)
            {
                Renderer.RenderOpagueSprite(blockSprite, block.Position, blocks.GridSize);
            }

            Renderer.RenderRectangle(crate.Position, crate.Size, Color.Blue);
            Renderer.RenderRectangle(crate2.Position, crate2.Size, Color.Blue);

            foreach (var player in players.Where(p => p.Active))
            {
                Renderer.RenderOpagueSprite(player.Sprite, player.Position, player.Size);
            }

            foreach (var projectile in projectiles)
            {
                Renderer.RenderRectangle(projectile.Position, projectile.Size, Color.Red);
            }
        }
    }
}