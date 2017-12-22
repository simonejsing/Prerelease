using Serialization;
using Terrain;

namespace CraftingGame
{
    public class TerrainFactory : ITerrainFactory
    {
        private int depth;
        private int height;
        private int seaLevel;
        private int seed;

        public TerrainFactory(int depth, int height, int seaLevel, int seed = 0)
        {
            this.depth = depth;
            this.height = height;
            this.seaLevel = seaLevel;
            this.seed = seed;
        }

        public ITerrainGenerator Create()
        {
            return new Generator(depth, height, seaLevel, seed);
        }

        public ITerrainGenerator FromState(StatefulObject state)
        {
            return Generator.FromState(state);
        }
    }
}