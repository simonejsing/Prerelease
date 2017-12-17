using System;
using System.Collections.Generic;
using System.Linq;
using Terrain;

namespace MainGame.UnitTests
{
    public struct TerrainPoint : IEquatable<TerrainPoint>
    {
        public readonly int X, Y, Z;

        public TerrainPoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + X;
            hash = hash * 31 + Y;
            hash = hash * 31 + Z;
            return hash;
        }

        public override bool Equals(object other)
        {
            return other is TerrainPoint ? Equals((TerrainPoint)other) : false;
        }

        public bool Equals(TerrainPoint other)
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }
    }

    public class TerrainStub : ITerrainGenerator
    {
        private readonly Dictionary<TerrainPoint, TerrainBlock> blocks = new Dictionary<TerrainPoint, TerrainBlock>();
        private int generationCounter = 0;

        public void AddBlock(int u, int v, int w, TerrainBlock block)
        {
            AddBlock(
                new Coordinate(u, v),
                new Plane(w),
                block);
        }

        public void AddBlock(Coordinate c, Plane p, TerrainBlock block)
        {
            block.Coord = c;
            blocks.Add(new TerrainPoint(c.U, c.V, p.W), block);
        }

        public int SeaLevel => 80;
        public int MaxDepth => 100;
        public int MaxHeight => 100;

        public TerrainBlock this[Coordinate c, Plane p]
        {
            get
            {
                var point = new TerrainPoint(c.U, c.V, p.W);
                generationCounter++;
                return blocks.FirstOrDefault(b => b.Key.Equals(point)).Value;
            }
        }

        public void Generate(Coordinate c, Plane p)
        {
        }

        public int GenerationCounter()
        {
            return generationCounter;
        }
    }
}
