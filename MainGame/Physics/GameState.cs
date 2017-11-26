using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class LevelState
    {
        public readonly string Name;

        private readonly List<EnemyObject> enemies = new List<EnemyObject>();
        private readonly List<Projectile> projectiles = new List<Projectile>();
        private readonly List<MovableObject> crates = new List<MovableObject>();
        private readonly List<Door> doors = new List<Door>();

        public IReadonlyVector SpawnPoint { get; }
        public BlockGrid Blocks { get; private set; }
        public IEnumerable<EnemyObject> Enemies => enemies;
        public IEnumerable<Projectile> Projectiles => projectiles;
        public IEnumerable<MovableObject> Crates => crates;
        public IEnumerable<Door> Doors => doors;

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
        }
    }

    public class GameState
    {
        // Global state
        private readonly PlayerObject[] players;

        // Per level state
        private readonly List<LevelState> levels = new List<LevelState>();

        public IEnumerable<PlayerObject> Players => players; 
        public IEnumerable<LevelState> Levels => levels;
        public LevelState ActiveLevel { get; private set; }

        public GameState(IEnumerable<PlayerObject> playerObject)
        {
            this.players = playerObject.ToArray();
        }

        public void SetActiveLevel(LevelState levelState)
        {
            var existingLevel = levels.FirstOrDefault(l => string.Equals(l.Name, levelState.Name));
            if (existingLevel != null)
            {
                ActiveLevel = existingLevel;
                return;
            }

            levels.Add(levelState);
            ActiveLevel = levelState;
        }
    }
}
