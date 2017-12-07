using System;
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
        private readonly IList<TerrainSector> sectors = new List<TerrainSector>();
        private TerrainSector activeSector;

        public IEnumerable<TerrainSector> Sectors => sectors;

        public CachedTerrainGenerator(ITerrainGenerator terrainGenerator)
        {
            this.terrainGenerator = terrainGenerator;
        }

        public TerrainBlock this[int x, int y, int z]
        {
            get
            {
                activeSector = FindSector(x, y, z);

                // Convert to sector u,v coordinates
                var u = x - activeSector.U * TerrainSector.SectorWidth;
                var v = y - activeSector.V * TerrainSector.SectorHeight;
                return activeSector[u, v];
            }
        }

        public void SetActiveSector(int x, int y, int z)
        {
            activeSector = FindSector(x, y, z);
        }

        public void Update(int numberOfTiles)
        {
            if (activeSector != null)
            {
                numberOfTiles -= activeSector.Update(numberOfTiles);

                // Continue to update nearby sectors
            }
        }

        public void Generate(int x, int y, int z)
        {
            activeSector = FindSector(x, y, z);

            // Convert to sector u,v coordinates
            var u = x - activeSector.U * TerrainSector.SectorWidth;
            var v = y - activeSector.V * TerrainSector.SectorHeight;
            activeSector.Generate(u, v);
        }

        private TerrainSector FindSector(int x, int y, int z)
        {
            var u = x / TerrainSector.SectorWidth;
            var v = y / TerrainSector.SectorHeight;

            // Handle negative coordinates
            if (Math.Sign(x) == -1)
                u--;
            if (Math.Sign(y) == -1)
                v--;

            if (activeSector?.U == u && activeSector?.V == v && activeSector?.W == z)
                return activeSector;

            var sector = Sectors.FirstOrDefault(s => s.U == u && s.V == v && s.W == z);
            if (sector == null)
            {
                sector = new TerrainSector(terrainGenerator, u, v, z);
                sectors.Add(sector);
            }

            return sector;
        }
    }
}
