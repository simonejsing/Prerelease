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
        private readonly Queue<TerrainSector> sectorLoadingQueue = new Queue<TerrainSector>();
        private TerrainSector activeLoadingSector;
        private QuadTree<TerrainSector> sectors = new QuadTree<TerrainSector>();

        public IEnumerable<TerrainSector> Sectors => sectors.Items;

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
                var sector = FindSector(c.U, c.V, p.W);
                var localCoord = sector.LocalCoordinate(c);
                return sector[localCoord.U, localCoord.V];
            }
        }

        public void SetActiveSector(int x, int y, int z)
        {
            var sector = FindSector(x, y, z);
            if (!sector.FullyLoaded)
            {
                sectorLoadingQueue.Enqueue(sector);
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
            var sector = FindSector(c.U, c.V, p.W);
            var localCoord = sector.LocalCoordinate(c);
            sector.Generate(localCoord.U, localCoord.V);
        }

        public void Destroy(Coordinate c, Plane p)
        {
            Modify(c, p, TerrainType.Free);
        }

        public void Place(Coordinate c, Plane p, TerrainType type)
        {
            Modify(c, p, type);
        }

        private void Modify(Coordinate c, Plane p, TerrainType type)
        {
            // Register modification
            terrainModifications[new Voxel(c, p)] = type;
            var sector = FindSector(c.U, c.V, p.W);
            var localCoord = sector.LocalCoordinate(c);
            sector.Modify(localCoord.U, localCoord.V, type);
        }

        private TerrainSector FindSector(int x, int y, int z)
        {
            // Handle negative coordinates
            var index = SectorIndex(x, y);
            return GetSector(new Voxel(index, new Plane(z)));
        }

        public TerrainSector GetSector(Voxel index)
        {
            var sector = sectors[index];
            if(sector == null)
            {
                sector = new TerrainSector(terrainGenerator, index);
                sectors.Add(index, sector);
                sectorLoadingQueue.Enqueue(sector);
            }
            return sector;
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
