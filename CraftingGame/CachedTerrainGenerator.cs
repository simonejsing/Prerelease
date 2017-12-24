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
        private readonly Dictionary<Voxel, TerrainType> terrainModifications = new Dictionary<Voxel, TerrainType>();
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
            Modify(c, p, TerrainType.Free);
        }

        private void Modify(Coordinate c, Plane p, TerrainType type)
        {
            // Register modification
            terrainModifications[new Voxel(c, p)] = type;

            activeSector = FindSector(c.U, c.V, p.W);

            var u = c.U - activeSector.U * TerrainSector.SectorWidth;
            var v = c.V - activeSector.V * TerrainSector.SectorHeight;
            activeSector.Modify(u, v, type);
        }

        private TerrainSector FindSector(int x, int y, int z)
        {
            // Handle negative coordinates
            var index = SectorIndex(x, y);

            if (activeSector?.U == index.U && activeSector?.V == index.V && activeSector?.W == z)
                return activeSector;

            var sector = Sectors.FirstOrDefault(s => s.U == index.U && s.V == index.V && s.W == z);
            if (sector == null)
            {
                sector = new TerrainSector(terrainGenerator, index.U, index.V, z);
                sectors.Add(sector);
                sectorLoadingQueue.Enqueue(sector);
            }

            return sector;
        }

        public Coordinate SectorPosition(Coordinate index)
        {
            return new Coordinate(index.U * TerrainSector.SectorWidth, index.V * TerrainSector.SectorHeight);
        }

        public Coordinate SectorIndex(Coordinate coord)
        {
            return SectorIndex(coord.U, coord.V);
        }

        public Coordinate SectorIndex(int u, int v)
        {
            return new Coordinate(
                DivideAndRoundDown(u, TerrainSector.SectorWidth),
                DivideAndRoundDown(v, TerrainSector.SectorHeight));
        }

        private int DivideAndRoundDown(int numerator, int denominator)
        {
            if(numerator >= 0)
                return numerator/denominator;
            return (numerator-denominator+1)/denominator;
        }

        public void ExtractState(StatefulObjectBuilder builder)
        {
            builder.Add("m", terrainModifications.Select(EncodeModification));
            builder.EmbedState("c.t", terrainGenerator);
        }

        private Dictionary<string, object> EncodeModification(KeyValuePair<Voxel, TerrainType> item)
        {
            return new Dictionary<string, object>
            {
                { "u", item.Key.U },
                { "v", item.Key.V },
                { "w", item.Key.W },
                { "t", item.Value },
            };
        }

        private static KeyValuePair<Voxel, TerrainType> DecodeModification(StatefulObject state)
        {
            var u = state.ReadMandatoryState<int>("u");
            var v = state.ReadMandatoryState<int>("v");
            var w = state.ReadMandatoryState<int>("w");
            var type = state.ReadMandatoryState<int>("t");
            return new KeyValuePair<Voxel, TerrainType>(new Voxel(u, v, w), (TerrainType)type);
        }

        internal static CachedTerrainGenerator FromState(ITerrainFactory terrainFactory, StatefulObject state)
        {
            var terrainState = state.ReadEmbeddedState("c.t");
            var self = new CachedTerrainGenerator(terrainFactory.FromState(terrainState));

            // Apply modifications
            var modifications = state.SafeReadList("m");
            foreach(var modification in modifications.Select(DecodeModification))
            {
                self.Modify(modification.Key.Coordinate, modification.Key.Plane, modification.Value);
            }

            return self;
        }
    }
}
