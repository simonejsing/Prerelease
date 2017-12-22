using Contracts;
using CraftingGame.State;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;

namespace CraftingGame
{
    public class CachedTerrainGenerator : IModifiableTerrain
    {
        private readonly ITerrainGenerator terrainGenerator;
        private readonly IList<TerrainSector> sectors = new List<TerrainSector>();
        private readonly Queue<TerrainSector> sectorLoadingQueue = new Queue<TerrainSector>();
        private TerrainSector activeLoadingSector;
        private TerrainSector activeSector;

        public IEnumerable<TerrainSector> Sectors => sectors;

        public CachedTerrainGenerator(ITerrainGenerator terrainGenerator)
        {
            this.terrainGenerator = terrainGenerator;
        }

        public int Seed => terrainGenerator.Seed;
        public int SeaLevel => terrainGenerator.SeaLevel;
        public int MaxDepth => terrainGenerator.MaxDepth;
        public int MaxHeight => terrainGenerator.MaxHeight;

        public Guid Id { get; set; }

        public TerrainBlock this[Coordinate c, Plane p]
        {
            get
            {
                activeSector = FindSector(c.U, c.V, p.W);

                // Convert to sector u,v coordinates
                var u = c.U - activeSector.U * TerrainSector.SectorWidth;
                var v = c.V - activeSector.V * TerrainSector.SectorHeight;
                return activeSector[u, v];
            }
        }

        public void SetActiveSector(int x, int y, int z)
        {
            activeSector = FindSector(x, y, z);
            if (!activeSector.FullyLoaded)
            {
                sectorLoadingQueue.Enqueue(activeSector);
            }
        }

        public void Update(int numberOfTiles)
        {
            if(numberOfTiles == -1)
            {
                numberOfTiles = Int32.MaxValue;
            }

            if (activeLoadingSector == null)
            {
                activeLoadingSector = GetNextLoadingSector();
            }

            while (activeLoadingSector != null && numberOfTiles > 0)
            {
                numberOfTiles -= activeLoadingSector.Update(numberOfTiles);
                if (activeLoadingSector.FullyLoaded)
                {
                    activeLoadingSector = GetNextLoadingSector();
                }
            }
        }

        private TerrainSector GetNextLoadingSector()
        {
            return sectorLoadingQueue.Any() ? sectorLoadingQueue.Dequeue() : null;
        }

        public void Generate(Coordinate c, Plane p)
        {
            activeSector = FindSector(c.U, c.V, p.W);

            // Convert to sector u,v coordinates
            var u = c.U - activeSector.U * TerrainSector.SectorWidth;
            var v = c.V - activeSector.V * TerrainSector.SectorHeight;
            activeSector.Generate(u, v);
        }

        public void Destroy(Coordinate c, Plane p)
        {
            activeSector = FindSector(c.U, c.V, p.W);

            var u = c.U - activeSector.U * TerrainSector.SectorWidth;
            var v = c.V - activeSector.V * TerrainSector.SectorHeight;
            activeSector.Destroy(u, v);
        }

        private TerrainSector FindSector(int x, int y, int z)
        {
            // Handle negative coordinates
            var u = DivideAndRoundDown(x, TerrainSector.SectorWidth);
            var v = DivideAndRoundDown(y, TerrainSector.SectorHeight);

            if (activeSector?.U == u && activeSector?.V == v && activeSector?.W == z)
                return activeSector;

            var sector = Sectors.FirstOrDefault(s => s.U == u && s.V == v && s.W == z);
            if (sector == null)
            {
                sector = new TerrainSector(terrainGenerator, u, v, z);
                sectors.Add(sector);
                sectorLoadingQueue.Enqueue(sector);
            }

            return sector;
        }

        private int DivideAndRoundDown(int numerator, int denominator)
        {
            if(numerator >= 0)
                return numerator/denominator;
            return (numerator-denominator+1)/denominator;
        }

        public IDictionary<string, object> ExtractState()
        {
            return new Dictionary<string, object>
            {
                { "c.t", terrainGenerator.ExtractState() }
            };
        }

        internal static CachedTerrainGenerator FromState(ITerrainFactory terrainFactory, StatefulObject state)
        {
            var subState = state.ReadEmbeddedState("c.t");
            return new CachedTerrainGenerator(terrainFactory.FromState(subState));
        }
    }
}
