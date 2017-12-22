using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class TerrainInverter : ITerrainGenerator
    {
        private static readonly Coordinate Inv = new Coordinate(1, -1);
        private readonly ITerrainGenerator generator;

        public int Seed => generator.Seed;
        public int SeaLevel => generator.SeaLevel;
        public int MaxDepth => generator.MaxDepth;
        public int MaxHeight => generator.MaxHeight;

        public Guid Id => generator.Id;

        public TerrainInverter(ITerrainGenerator generator)
        {
            this.generator = generator;
        }

        public TerrainBlock this[Coordinate c, Plane p] => generator[c * Inv, p];

        public void Generate(Coordinate c, Plane p)
        {
            generator.Generate(c * Inv, p);
        }

        public IDictionary<string, object> ExtractState()
        {
            return generator.ExtractState();
        }
    }
}
