using System.Collections.Generic;
using System.Linq;
using Contracts;
using Prerelease.Main.Physics;

namespace Prerelease.Main
{
    public class ObjectManager
    {
        private readonly Dictionary<int, IEnumerable<IRenderableObject>> renderLayers = new Dictionary<int, IEnumerable<IRenderableObject>>();
        private readonly Dictionary<int, IEnumerable<ICollidableObject>> collisionLayers = new Dictionary<int, IEnumerable<ICollidableObject>>();
        private readonly List<PlayerObject> players;

        public BlockGrid Blocks { get; private set; }
        private IEnumerable<PlayerObject> ActivePlayers => players.Where(p => p.Active && !p.Dead);
        public IEnumerable<IRenderableObject> RenderOrder => renderLayers.SelectMany(objs => objs.Value).Concat(ActivePlayers);
        public IEnumerable<ICollidableObject> CollisionOrder => collisionLayers.SelectMany(objs => objs.Value).Concat(ActivePlayers);

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
            renderLayers.Add(2, activeLevel.Crates);
            renderLayers.Add(3, activeLevel.Enemies);
            //layers.Add(4, activeLevel.Projectiles);

            collisionLayers.Clear();
            collisionLayers.Add(0, activeLevel.Crates.Concat(activeLevel.StaticObjects));
            collisionLayers.Add(1, activeLevel.Enemies);
        }
    }
}