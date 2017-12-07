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
        public const int SectorHeight = 1000;
        public const int SectorWidth = 1000;

        private readonly ITerrainPlane plane;
        private int updateCount = 0;

        public int X { get; }
        public int Y { get; }
        public TerrainBlock[,] Tiles { get; }
        public bool FullyLoaded { get; private set; }

        // Update a given number of tiles
        public void Update(int numberOfTiles)
        {
            var numberUpdates = Math.Min(SectorWidth * SectorHeight - updateCount, numberOfTiles);
            for (int i = 0; i < numberUpdates; i++)
            {
                var y = updateCount / SectorWidth;
                var x = updateCount % SectorWidth;
                Generate(x, y);
                updateCount++;
            }

            FullyLoaded = updateCount == SectorWidth * SectorHeight;
        }

        public TerrainSector(ITerrainPlane plane, int x, int y)
        {
            this.plane = plane;
            FullyLoaded = false;
            X = x;
            Y = y;
            Tiles = new TerrainBlock[SectorWidth, SectorHeight];
            for (int sy = 0; sy < SectorHeight; sy++)
            {
                for (int sx = 0; sx < SectorWidth; sx++)
                {
                    Tiles[sx,sy] = new TerrainBlock() { Type = TerrainType.NotGenerated, X = X * SectorWidth + sx, Y = Y * SectorHeight + sy };
                }
            }
        }

        public void Generate(int x, int y)
        {
            Tiles[x, y] = plane[X * SectorWidth + x, Y * SectorHeight + y];
        }
    }
}