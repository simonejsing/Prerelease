using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class TerrainInverter : ITerrainGenerator
    {
        private readonly ITerrainGenerator generator;
        private readonly int height;

        public int SeaLevel => generator.SeaLevel;
        public int MaxDepth => generator.MaxDepth;
        public int MaxHeight => generator.MaxHeight;

        public TerrainInverter(ITerrainGenerator generator)
        {
            this.generator = generator;
            this.height = generator.MaxDepth;
        }

        public TerrainBlock this[int x, int y, int z] => generator[x, -y, z];

        public void Generate(int x, int y, int z)
        {
            generator.Generate(x, -y, z);
        }
    }
}
