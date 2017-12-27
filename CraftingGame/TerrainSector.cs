using Contracts;
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
        private readonly TerrainBlock[,] tiles;
        private int updateCount = 0;

        private readonly Coordinate bottomLeft;

        public Voxel Index { get; }
        public int U => Index.U;
        public int V => Index.V;
        public int W => Index.W;
        public bool FullyLoaded { get; private set; }

        public TerrainBlock this[int u, int v] => tiles[u, v];

        //public event TerrainModificationEventHandler TerrainModification;
        public event EventHandler<TerrainModificationEvent> TerrainModification;

        public Coordinate GlobalCoordinate(Coordinate localCoord)
        {
            return new Coordinate(localCoord.U * SectorWidth, localCoord.V * SectorHeight);
        }

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
            Index = new Voxel(u, v, w);
            this.bottomLeft = new Coordinate(U, V) * new Coordinate(SectorWidth, SectorHeight);
            tiles = new TerrainBlock[SectorWidth, SectorHeight];
            for (int y = 0; y < SectorHeight; y++)
            {
                for (int x = 0; x < SectorWidth; x++)
                {
                    tiles[x,y] = new TerrainBlock()
                    {
                        Coord = new Coordinate(U * SectorWidth + x, V * SectorHeight + y),
                        Type = TerrainType.NotGenerated,
                    };
                }
            }
        }

        public void Generate(int u, int v)
        {
            if (tiles[u, v].Type == TerrainType.NotGenerated)
            {
                tiles[u, v] = generator[this.bottomLeft + new Coordinate(u, v), this.Index.Plane];
            }
        }

        public void Modify(int u, int v, TerrainType type)
        {
            tiles[u, v].Type = type;
            OnTerrainModified(new Coordinate(u, v));
        }

        private void OnTerrainModified(Coordinate localCoord)
        {
            TerrainModification?.Invoke(this, new TerrainModificationEvent(Index, localCoord, this.bottomLeft + localCoord));
        }
    }
}