using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public enum TerrainType { NotGenerated = -1, Free = 0, Dirt = 1, Rock = 2, Bedrock = 3, Sea = 4 }

    public struct TerrainBlock
    {
        public int X, Y;
        public TerrainType Type;
    }

}
