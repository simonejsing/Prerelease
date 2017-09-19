using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private BlockGrid blocks;
        private readonly List<Projectile> projectiles = new List<Projectile>();
        private readonly List<MovableObject> crates = new List<MovableObject>();
        private readonly PhysicsEngine physics;
        private ISprite blockSprite, crateSprite;

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

            CreateLevel();

            physics = new PhysicsEngine(blocks, UpdateStep);
            foreach (var crate in crates)
            {
                physics.AddMovableObject(crate);
            }
        }

        private void CreateLevel()
        {
            var text = ReadLevelBlocks("Level1");

            var lines = text.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            blocks = new BlockGrid(30, 30, (uint)lines.Length, (uint)lines[0].Length);

            for (uint row = 0; row < lines.Length; row++)
            {
                if(lines[row].Length > blocks.Columns)
                    throw new InvalidDataException($"Row number {row} has an invalid column count {lines[row].Length}, it must not be greater than {blocks.Columns}.");

                for (uint col = 0; col < lines[row].Length; col++)
                {
                    switch (lines[row][(int)col])
                    {
                        case 'x':
                        case 'X':
                            blocks.Insert(row, col);
                            break;
                        case 'c':
                        case 'C':
                            crates.Add(new MovableObject(actionQueue, new Vector2(30 * col, 30 * row), new Vector2(30, 30)));
                            break;
                    }
                }
            }
        }

        private string ReadLevelBlocks(string levelName)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var resourceName = $"Prerelease.Main.Maps.{levelName}.blocks.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public override void Activate()
        {
            base.Activate();

            // Load content in active scope.
            blockSprite = LoadSprite("Block");
            crateSprite = LoadSprite("Crate");
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
            foreach (var crate in crates)
            {
                physics.ApplyToObject(crate, ZeroVector);
            }

            foreach (var projectile in projectiles)
            {
                physics.ApplyToProjectile(projectile);
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

            if (player.OnGround)
            {
                if (inputMask.Input.Up)
                {
                    instantVelocity.Y = Constants.JUMP_SPEED;
                }
            }

            var horizontalControl = player.OnGround ? Constants.GROUND_ACCELERATION : Constants.AIR_ACCELERATION;
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
            if (player.OnGround && !horizontalInput)
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

            foreach (var block in blocks.OccupiedBlocks)
            {
                Renderer.RenderOpagueSprite(blockSprite, block.Position, blocks.GridSize);
            }

            foreach (var crate in crates)
            {
                Renderer.RenderOpagueSprite(crateSprite, crate.Position, crate.Size);
            }

            foreach (var player in players.Where(p => p.Active))
            {
                Renderer.RenderOpagueSprite(player.Sprite, player.Position, player.Size, player.Facing.X < 0);
            }

            foreach (var projectile in projectiles)
            {
                Renderer.RenderRectangle(projectile.Position, projectile.Size, Color.Red);
            }
        }
    }
}