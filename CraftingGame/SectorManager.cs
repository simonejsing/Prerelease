using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;

namespace CraftingGame
{
    /*
     * A sector is a fixed size constant index time representation of a block
     * of terrain.
     */
    public struct Sector<T>
    {
        public const int SectorHeight = 1000;
        public const int SectorWidth = 1000;

        public int X { get; }
        public int Y { get; }
        public T[,] Tiles;

        internal Sector(int x, int y)
        {
            X = x;
            Y = y;
            Tiles = new T[SectorWidth, SectorHeight];
        }
    }

    public class SectorManager
    {
        private readonly ITerrainGenerator terrainGenerator;

        public SectorManager(ITerrainGenerator terrainGenerator)
        {
            this.terrainGenerator = terrainGenerator;
        }

        public Sector<TerrainBlock> GetSector(int x, int y)
        {
            return new Sector<TerrainBlock>(x, y);
        }
    }
}
