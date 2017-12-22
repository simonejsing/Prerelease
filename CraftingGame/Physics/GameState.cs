﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VectorMath;

namespace CraftingGame.Physics
{
    public class LevelState
    {
        public readonly string Name;

        private readonly List<EnemyObject> enemies = new List<EnemyObject>();
        private readonly List<Projectile> projectiles = new List<Projectile>();
        private readonly List<MovableObject> crates = new List<MovableObject>();
        private readonly List<Door> doors = new List<Door>();
        private readonly List<StaticObject> staticObjects = new List<StaticObject>();
        private readonly List<ItemObject> collectableObjects = new List<ItemObject>();

        public IReadonlyVector SpawnPoint { get; }
        public BlockGrid Blocks { get; private set; }

        // Objects
        public IEnumerable<EnemyObject> Enemies => enemies;
        public IEnumerable<Projectile> Projectiles => projectiles;
        public IEnumerable<MovableObject> Crates => crates;
        public IEnumerable<Door> Doors => doors;
        public IEnumerable<StaticObject> StaticObjects => staticObjects;
        public IEnumerable<ItemObject> CollectableObjects => collectableObjects;

        public IEnumerable<Object> AllObjects => ((IEnumerable<Object>)enemies).Concat(projectiles).Concat(crates).Concat(doors).Concat(staticObjects).Concat(collectableObjects);

        public LevelState(string name, IReadonlyVector spawnPoint)
        {
            SpawnPoint = spawnPoint;
            Name = name;
        }

        public void SetBlocks(BlockGrid blockGrid)
        {
            Blocks = blockGrid;
        }

        public void AddEnemies(IEnumerable<EnemyObject> levelEnemies)
        {
            enemies.AddRange(levelEnemies);
        }

        public void AddCrates(IEnumerable<MovableObject> levelCrates)
        {
            crates.AddRange(levelCrates);
        }

        public void AddDoors(IEnumerable<Door> levelDoors)
        {
            doors.AddRange(levelDoors);
        }

        public void AddStaticObjects(IEnumerable<StaticObject> levelStaticObjects)
        {
            staticObjects.AddRange(levelStaticObjects);
        }

        public void AddCollectableObjects(params ItemObject[] levelCollectableObjects)
        {
            collectableObjects.AddRange(levelCollectableObjects);
        }

        public void AddProjectiles(params Projectile[] projectile)
        {
            projectiles.AddRange(projectile);
        }

        public void CleanUp()
        {
            // Delete expired projectiles
            projectiles.RemoveAll(p => p.Expired);

            // Delete dead enemies
            enemies.RemoveAll(e => e.Dead);

            // Remove collected items
            collectableObjects.RemoveAll(c => c.Collected);
        }
    }

    public class GameState
    {
        // Global state
        private PlayerObject[] players;

        // Per level state
        private readonly List<LevelState> levels = new List<LevelState>();

        public IEnumerable<PlayerObject> Players => players;
        public IEnumerable<PlayerObject> ActivePlayers => Players.Where(p => p.Active);
        public IEnumerable<LevelState> Levels => levels;
        public LevelState ActiveLevel { get; private set; }

        public Door DoorToEnter { get; set; }

        public GameState()
        {
            this.players = new PlayerObject[0];
        }

        public void AddPlayer(PlayerObject player)
        {
            if(this.players.Any(p => p.PlayerBinding == player.PlayerBinding))
            {
                throw new InvalidOperationException($"Attempt to add player that already exists in state '{player.PlayerBinding}'.");
            }
            this.players = this.players.Concat(new[] { player }).ToArray();
        }

        public void AddLevel(LevelState level)
        {
            levels.Add(level);
        }

        public void SetActiveLevel(string levelName)
        {
            ActiveLevel = levels.First(l => string.Equals(l.Name, levelName));
        }

        public void SaveToStream(Stream stream)
        {
        }

        public void LoadFromStream(Stream stream)
        {
        }
    }
}
