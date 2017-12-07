using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class TerrainPlane : ITerrainPlane
    {
        private readonly ITerrainGenerator generator;
        public int Plane { get; }

        public TerrainPlane(ITerrainGenerator generator, int plane)
        {
            this.generator = generator;
            this.Plane = plane;
        }

        public TerrainBlock this[int x, int y] => generator[x, y, Plane];
    }
}
