using System;
using System.Collections.Generic;
using System.Linq;
using Terrain;

namespace MainGame.UnitTests
{
    public struct TerrainPoint : IEquatable<TerrainPoint>
    {
        public int X, Y, Z;

        public TerrainPoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = y;
        }

        public bool Equals(TerrainPoint other)
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }
    }

    public class TerrainStub : ITerrainGenerator
    {
        private readonly Dictionary<TerrainPoint, TerrainBlock> blocks = new Dictionary<TerrainPoint, TerrainBlock>();

        public void AddBlock(int x, int y, int z, TerrainBlock block)
        {
            block.X = x;
            block.Y = y;
            blocks.Add(new TerrainPoint(x, y, z), block);
        }

        public TerrainBlock this[int x, int y, int z]
        {
            get
            {
                var p = new TerrainPoint(x, y, z);
                return blocks.FirstOrDefault(b => b.Key.Equals(p)).Value;
            }
        }

        public ITerrainPlane Plane(int z)
        {
            return new TerrainPlane(this, z);
        }
    }
}
