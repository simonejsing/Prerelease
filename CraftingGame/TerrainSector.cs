using System;
using Terrain;

namespace CraftingGame
{
    /*
     * A sector is a fixed size constant index time representation of a block
     * of terrain.
     */
    public class TerrainSector
    {
        public const int SectorWidth = 100;
        public const int SectorHeight = 100;

        private readonly ITerrainGenerator generator;
        private int updateCount = 0;
        private TerrainBlock[,] Tiles { get; }

        public int U { get; }
        public int V { get; }
        public int W { get; }
        public bool FullyLoaded { get; private set; }

        public TerrainBlock this[int u, int v] => Tiles[u, v];

        // Update a given number of tiles
        public int Update(int numberOfTiles)
        {
            if (FullyLoaded)
                return 0;

            var originalCount = updateCount;
            var numberUpdates = SectorWidth * SectorHeight - updateCount;
            if (numberOfTiles != -1)
            {
                numberUpdates = Math.Min(numberUpdates, numberOfTiles);
            }

            for (int i = 0; i < numberUpdates; i++)
            {
                var y = updateCount / SectorWidth;
                var x = updateCount % SectorWidth;
                Generate(x, y);
                updateCount++;
            }

            FullyLoaded = updateCount == SectorWidth * SectorHeight;
            return updateCount - originalCount;
        }

        public TerrainSector(ITerrainGenerator generator, int u, int v, int w)
        {
            this.generator = generator;
            FullyLoaded = false;
            U = u;
            V = v;
            W = w;
            Tiles = new TerrainBlock[SectorWidth, SectorHeight];
            for (int y = 0; y < SectorHeight; y++)
            {
                for (int x = 0; x < SectorWidth; x++)
                {
                    Tiles[x,y] = new TerrainBlock() { Type = TerrainType.NotGenerated, X = U * SectorWidth + x, Y = V * SectorHeight + y };
                }
            }
        }

        public void Generate(int u, int v)
        {
            Tiles[u, v] = generator[U * SectorWidth + u, V * SectorHeight + v, W];
        }
    }
}