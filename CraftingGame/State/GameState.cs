using Contracts;
using CraftingGame.Items;
using CraftingGame.Physics;
using CraftingGame.State.Upgrade;
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
        private readonly ItemObject[] collectableObjects = new ItemObject[20];
        private int collectableObjectIndex = 0;

        public IReadonlyVector SpawnPoint { get; }
        public BlockGrid Blocks { get; private set; }

        // Objects
        public IEnumerable<EnemyObject> Enemies => enemies;
        public IEnumerable<Projectile> Projectiles => projectiles;
        public IEnumerable<MovableObject> Crates => crates;
        public IEnumerable<Door> Doors => doors;
        public IEnumerable<StaticObject> StaticObjects => staticObjects;
        public IEnumerable<ItemObject> CollectableObjects => collectableObjects.Where(o => o != null);

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
            foreach(var obj in levelCollectableObjects)
            {
                collectableObjects[collectableObjectIndex] = obj;
                collectableObjectIndex = (collectableObjectIndex + 1) % collectableObjects.Length;
            }
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
            for(var i = 0; i < collectableObjects.Length; i++)
            {
                if (collectableObjects[i]?.Collected ?? false)
                {
                    collectableObjects[i] = null;
                }
            }
        }
    }

    public class GameState
    {
        public const int LatestVersion = 2;

        private readonly ITerrainFactory terrainFactory;
        private readonly List<PlayerObject> boundPlayers = new List<PlayerObject>();

        // Global state
        private Dictionary<string, PlayerObject> knownPlayers;

        // Per level state
        private readonly List<LevelState> levels = new List<LevelState>();

        public int Version { get; private set; }
        public ActionQueue ActionQueue { get; }
        public Grid Grid { get; }
        public CachedTerrainGenerator Terrain { get; set; }
        public ItemFactory ItemFactory { get; }

        public IEnumerable<PlayerObject> KnownPlayers => knownPlayers.Values;

        public IEnumerable<PlayerObject> BoundPlayers => boundPlayers;
        public IEnumerable<PlayerObject> ActivePlayers => boundPlayers.Where(p => p.Active);
        public IEnumerable<LevelState> Levels => levels;
        public LevelState ActiveLevel { get; private set; }

        public static GameState LoadFromStream(ActionQueue actionQueue, Grid grid, ITerrainFactory terrainFactory, Stream stream)
        {
            var state = SerializableState.FromStream(stream);
            StatefulObject globalState = ReadGlobalSection(actionQueue, state);
            var version = globalState?.SafeReadValue("version", 1) ?? 1;
            var gameState = Create(actionQueue, grid, terrainFactory, version);

            // Load players
            gameState.knownPlayers = gameState.LoadEntities(state, "players", PlayerObject.FromState).ToDictionary(p => p.PlayerBinding, p => p);

            foreach (var player in gameState.knownPlayers.Values)
            {
                EquipPlayerItems(gameState, player);
            }

            // Load terrain
            gameState.Terrain = gameState.LoadEntities(state, "terrain", s => CachedTerrainGenerator.FromState(terrainFactory, s)).First();

            // Apply upgrade rules
            var rules = new IGameStateUpgradeRule[] { new GameStateUpgradeV2() };
            UpgradeGameState(gameState, rules);

            return gameState;
        }

        private static void UpgradeGameState(GameState state, IGameStateUpgradeRule[] rules)
        {
            while(state.Version != LatestVersion)
            {
                // Find the next best rule
                var nextRule = rules.Where(r => r.FromVersion == state.Version).OrderByDescending(r => r.ToVersion).First();
                nextRule.Upgrade(state);
                state.Version = nextRule.ToVersion;
            }
        }

        private static StatefulObject ReadGlobalSection(ActionQueue actionQueue, SerializableState state)
        {
            return new StatefulObject(actionQueue, new Guid(), state.Global);
        }

        public static GameState Create(ActionQueue actionQueue, Grid grid, ITerrainFactory terrainFactory)
        {
            return Create(actionQueue, grid, terrainFactory, LatestVersion);
        }

        private static GameState Create(ActionQueue actionQueue, Grid grid, ITerrainFactory terrainFactory, int version)
        {
            return new GameState(actionQueue, grid, terrainFactory, version);
        }

        private GameState(ActionQueue actionQueue, Grid grid, ITerrainFactory terrainFactory, int currentVersion)
        {
            this.Version = currentVersion;
            this.ActionQueue = actionQueue;
            this.Grid = grid;
            this.terrainFactory = terrainFactory;
            this.ItemFactory = new ItemFactory(this);
            this.knownPlayers = new Dictionary<string, PlayerObject>();

            // Start new game
            Terrain = new CachedTerrainGenerator(terrainFactory.Create());
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
            state.AddGlobalProperty("version", Version);
            state.AddEntities("players", KnownPlayers);
            state.AddEntities("terrain", new[] { Terrain });
            state.Serialize(stream);
        }

        private T[] LoadEntities<T>(SerializableState state, string entityType, Func<StatefulObject, T> factory)
        {
            // Are there any?
            if (!state.State.ContainsKey(entityType))
            {
                return new T[0];
            }

            return state.State[entityType].Select(e => factory(new StatefulObject(ActionQueue, e.Key, e.Value))).ToArray();
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
            var player = new PlayerObject(ActionQueue)
            {
                Id = Guid.NewGuid(),
                PlayerBinding = inputMask.PlayerBinding,
                SpriteBinding = new ObjectBinding<ISprite>("Chicken"),
                Plane = new Plane(0),
                Size = new Vector2(30, 30),
                Color = Color.Red
            };

            EquipPlayerItems(this, player);
            player.BindInput(inputMask);
            boundPlayers.Add(player);
            return player;
        }

        private static void EquipPlayerItems(GameState state, PlayerObject player)
        {
            player.AddEquipment(new PickAxe(state));
            player.AddEquipment(new PlacableTerrainBlock(state, TerrainType.Dirt));
            player.AddEquipment(new PlacableTerrainBlock(state, TerrainType.Rock));
        }
    }
}
