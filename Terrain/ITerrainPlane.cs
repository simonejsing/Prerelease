using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public interface ITerrainPlane
    {
        TerrainBlock this[int x, int y] { get; }
    }
}
