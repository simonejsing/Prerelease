using Contracts;
using CraftingGame.Physics;
using Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terrain;
using VectorMath;
using Object = CraftingGame.Physics.Object;

namespace CraftingGame.State
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
        private readonly ActionQueue actionQueue;
        private readonly ITerrainFactory terrainFactory;
        private readonly List<PlayerObject> boundPlayers = new List<PlayerObject>();

        // Global state
        private Dictionary<string, PlayerObject> knownPlayers;

        // Per level state
        private readonly List<LevelState> levels = new List<LevelState>();

        public CachedTerrainGenerator Terrain { get; set; }

        public IEnumerable<PlayerObject> KnownPlayers => knownPlayers.Values;

        public IEnumerable<PlayerObject> BoundPlayers => boundPlayers;
        public IEnumerable<PlayerObject> ActivePlayers => boundPlayers.Where(p => p.Active);
        public IEnumerable<LevelState> Levels => levels;
        public LevelState ActiveLevel { get; private set; }

        public GameState(ActionQueue actionQueue, ITerrainFactory terrainFactory)
        {
            this.actionQueue = actionQueue;
            this.terrainFactory = terrainFactory;
            this.knownPlayers = new Dictionary<string, PlayerObject>();
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
            var state = new SerializableState();
            state.AddEntities("players", KnownPlayers);
            state.AddEntities("terrain", new[] { Terrain });
            state.Serialize(stream);
        }

        public void LoadFromStream(Stream stream)
        {
            var state = SerializableState.FromStream(stream);

            // Load players
            knownPlayers = LoadEntities(state, "players", PlayerObject.FromState).ToDictionary(p => p.PlayerBinding, p => p);

            // Load terrain
            Terrain = LoadEntities(state, "terrain", s => CachedTerrainGenerator.FromState(terrainFactory, s)).First();
        }

        private T[] LoadEntities<T>(SerializableState state, string entityType, Func<StatefulObject, T> factory)
        {
            // Are there any?
            if (!state.State.ContainsKey(entityType))
            {
                return new T[0];
            }

            return state.State[entityType].Select(e => factory(new StatefulObject(actionQueue, e.Key, e.Value))).ToArray();
        }

        internal IEnumerable<PlayerObject> BindPlayers(IEnumerable<InputMask> unboundControls)
        {
            foreach (var inputMask in unboundControls)
            {
                // See if there exists a matching player
                PlayerObject existingPlayer = null;
                if (!knownPlayers.TryGetValue(inputMask.PlayerBinding, out existingPlayer))
                {
                    // New player joined
                    var newPlayer = CreatePlayer(inputMask);
                    AddPlayer(newPlayer);
                    yield return newPlayer;
                }
                else if (!existingPlayer.InputBound)
                {
                    // Reconnect
                    existingPlayer.BindInput(inputMask);
                    boundPlayers.Add(existingPlayer);
                }
            }
        }

        private void AddPlayer(PlayerObject player)
        {
            this.knownPlayers.Add(player.PlayerBinding, player);
        }

        private PlayerObject CreatePlayer(InputMask inputMask)
        {
            var player = new PlayerObject(actionQueue)
            {
                Id = Guid.NewGuid(),
                PlayerBinding = inputMask.PlayerBinding,
                SpriteBinding = new ObjectBinding<ISprite>("Chicken"),
                Plane = new Plane(0),
                Size = new Vector2(30, 30),
                Color = Color.Red
            };

            player.BindInput(inputMask);
            boundPlayers.Add(player);
            return player;
        }
    }
}
