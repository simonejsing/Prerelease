using System.Collections.Generic;
using System.Linq;
using Contracts;
using CraftingGame.Physics;

namespace CraftingGame
{
    public class ObjectManager : IObjectManager
    {
        private readonly Dictionary<int, IEnumerable<IRenderableObject>> renderLayers = new Dictionary<int, IEnumerable<IRenderableObject>>();
        private readonly Dictionary<int, IEnumerable<ICollidableObject>> collisionLayers = new Dictionary<int, IEnumerable<ICollidableObject>>();
        private readonly Dictionary<int, IEnumerable<ICollectableObject>> collectableLayers = new Dictionary<int, IEnumerable<ICollectableObject>>();
        private readonly List<PlayerObject> players;

        public ICollidableObjectGrid Blocks { get; private set; }
        private IEnumerable<PlayerObject> ActivePlayers => players.Where(p => p.Active && !p.Dead);
        public IEnumerable<IRenderableObject> RenderOrder => renderLayers.SelectMany(objs => objs.Value).Concat(ActivePlayers);
        public IEnumerable<ICollidableObject> CollisionOrder => collisionLayers.SelectMany(objs => objs.Value).Concat(ActivePlayers);
        public IEnumerable<ICollectableObject> CollectableOrder => collectableLayers.SelectMany(objs => objs.Value.Where(o => o.Collected == false));

        public ObjectManager(List<PlayerObject> players)
        {
            this.players = players;
        }

        public void PartitionLevelObjects(LevelState activeLevel)
        {
            Blocks = activeLevel.Blocks;

            renderLayers.Clear();
            renderLayers.Add(0, activeLevel.Blocks.OccupiedBlocks);
            renderLayers.Add(1, ((IEnumerable<IRenderableObject>)activeLevel.StaticObjects).Concat(activeLevel.Doors));
            renderLayers.Add(2, ((IEnumerable<IRenderableObject>)activeLevel.CollectableObjects.Where(o => o.Collected == false)));
            renderLayers.Add(3, activeLevel.Crates);
            renderLayers.Add(4, activeLevel.Enemies);
            //layers.Add(5, activeLevel.Projectiles);

            collisionLayers.Clear();
            collisionLayers.Add(0, activeLevel.Crates.Concat(activeLevel.StaticObjects));
            collisionLayers.Add(1, activeLevel.Enemies);

            collectableLayers.Clear();
            collectableLayers.Add(0, activeLevel.CollectableObjects);
        }
    }
}