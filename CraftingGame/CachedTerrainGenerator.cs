using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;

namespace CraftingGame
{
    public class CachedTerrainGenerator : ITerrainGenerator
    {
        private readonly ITerrainGenerator terrainGenerator;

        public CachedTerrainGenerator(ITerrainGenerator terrainGenerator)
        {
            this.terrainGenerator = terrainGenerator;
        }

        private TerrainSector GetSector(int x, int y, int z)
        {
            return new TerrainSector(terrainGenerator.Plane(z), x, y);
        }

        public TerrainBlock this[int x, int y, int z]
        {
            get { throw new System.NotImplementedException(); }
        }

        public ITerrainPlane Plane(int z)
        {
            throw new System.NotImplementedException();
        }
    }
}
