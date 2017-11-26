using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private PhysicsEngine physics;
        private ISprite blockSprite;

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

            var level = CreateLevel("Level1");

            var players = new List<PlayerObject>
            {
                new PlayerObject(actionQueue, inputMasks[0], level.SpawnPoint, new Vector2(30, 30), Color.Red),
                new PlayerObject(actionQueue, inputMasks[1], level.SpawnPoint, new Vector2(30, 30), Color.Green),
                new PlayerObject(actionQueue, inputMasks[2], level.SpawnPoint, new Vector2(30, 30), Color.Blue),
                new PlayerObject(actionQueue, inputMasks[3], level.SpawnPoint, new Vector2(30, 30), Color.Yellow),
            };

            state = new GameState(players);
            physics = new PhysicsEngine(state, UpdateStep);

            state.SetActiveLevel(level);

            // Load content in active scope.
            blockSprite = LoadSprite("Block");
            foreach (var player in players)
            {
                player.Sprite = LoadSprite("Chicken");
                player.Sprite.Size = new Vector2(30, 30);
            }
        }

        private LevelState CreateLevel(string levelName)
        {
            var levelEnemies = new List<EnemyObject>();
            var levelCrates = new List<MovableObject>();
            var levelDoors = new List<Door>();
            var levelText = ReadLevelBlocks(levelName);

            var lines = levelText.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var blocks = new BlockGrid(30, 30, (uint)lines.Length, (uint)lines[0].Length);

            Vector2 spawnPoint = new Vector2();

            for (uint row = 0; row < lines.Length; row++)
            {
                if(lines[row].Length > blocks.Columns)
                    throw new InvalidDataException($"Row number {row} has an invalid column count {lines[row].Length}, it must not exceed {blocks.Columns}.");

                for (uint col = 0; col < lines[row].Length; col++)
                {
                    var cellValue = lines[row][(int) col];
                    switch (cellValue)
                    {
                        case 's':
                            spawnPoint = new Vector2(30*col, 30*row);
                            break;
                        case 'x':
                        case 'X':
                            blocks.Insert(row, col);
                            break;
                        case 'c':
                        case 'C':
                            var crate = CreateCrate(new Vector2(30*col, 30*row), new Vector2(30, 30));
                            levelCrates.Add(crate);
                            break;
                        case 'e':
                        case 'E':
                            var enemy = CreateEnemy(new Vector2(30 * col, 30 * row), new Vector2(30, 30));
                            levelEnemies.Add(enemy);
                            break;
                        case '1':
                        case '2':
                        case '3':
                            // Level door
                            int levelNumber = (int)char.GetNumericValue(cellValue);
                            var door = CreateDoorToLevel(new Vector2(30 * col, 30 * row), new Vector2(30, 30), levelNumber);
                            levelDoors.Add(door);
                            break;
                    }
                }
            }

            var level = new LevelState(levelName, spawnPoint);
            level.SetBlocks(blocks);
            level.AddEnemies(levelEnemies);
            level.AddCrates(levelCrates);
            level.AddDoors(levelDoors);
            return level;
        }

        private MovableObject CreateCrate(Vector2 position, Vector2 size)
        {
            var crate = new MovableObject(actionQueue, position, size);
            crate.Sprite = LoadSprite("crate");
            return crate;
        }

        private Door CreateDoorToLevel(Vector2 position, Vector2 size, int levelNumber)
        {
            var door = new Door(actionQueue, position, size);
            door.Sprite = LoadSprite("door");
            door.Destination = new Destination()
            {
                Type = DestinationType.Level,
                Identifier = levelNumber
            };
            return door;
        }

        private EnemyObject CreateEnemy(Vector2 position, Vector2 size)
        {
            var enemy = new EnemyObject(actionQueue, position, size);
            enemy.Sprite = LoadSprite("skeleton");
            return enemy;
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

        public override void Update(float timestep)
        {
            foreach(var player in state.Players)
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
                if (inputMask.Input.Up)
                {
                    // Enter door if on door sprite
                    var doorToEnter = state.ActiveLevel.Doors.FirstOrDefault(d => player.BoundingBox.Inside(d.Center));
                    if(doorToEnter != null)
                    {
                        // Enter the door
                    }

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

            if (inputMask.Input.Select)
            {
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

            var blocks = state.ActiveLevel.Blocks;
            foreach (var block in blocks.OccupiedBlocks)
            {
                Renderer.RenderOpagueSprite(blockSprite, block.Position, blocks.GridSize);
            }

            foreach (var door in state.ActiveLevel.Doors)
            {
                Renderer.RenderOpagueSprite(door.Sprite, door.Position, door.Size);
            }

            foreach (var crate in state.ActiveLevel.Crates)
            {
                Renderer.RenderOpagueSprite(crate.Sprite, crate.Position, crate.Size);
            }

            foreach (var enemy in state.ActiveLevel.Enemies)
            {
                Renderer.RenderOpagueSprite(enemy.Sprite, enemy.Position, enemy.Size, enemy.Facing.X < 0);
            }

            foreach (var player in state.Players.Where(p => p.Active && !p.Dead))
            {
                Renderer.RenderOpagueSprite(player.Sprite, player.Position, player.Size, player.Facing.X < 0);
            }

            foreach (var projectile in state.ActiveLevel.Projectiles)
            {
                Renderer.RenderRectangle(projectile.Position, projectile.Size, Color.Red);
            }
        }
    }
}