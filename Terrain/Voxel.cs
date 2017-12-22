using Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Terrain
{
    public struct Voxel : IEquatable<Voxel>
    {
        public Coordinate Coordinate;
        public Plane Plane;

        public int U => Coordinate.U;
        public int V => Coordinate.V;
        public int W => Plane.W;

        public Voxel(int u, int v, int w) : this(new Coordinate(u, v), new Plane(w))
        {
        }

        public Voxel(Coordinate c, Plane p)
        {
            Coordinate = c;
            Plane = p;
        }

        public override string ToString()
        {
            return $"({U},{V},{W})";
        }

        public static bool operator ==(Voxel self, Voxel other)
        {
            return self.Equals(other);
        }

        public static bool operator !=(Voxel self, Voxel other)
        {
            return !self.Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + U;
            hash = hash * 31 + V;
            hash = hash * 31 + W;
            return hash;
        }

        public override bool Equals(object other)
        {
            return other is Voxel ? Equals((Voxel)other) : false;
        }

        public bool Equals(Voxel other)
        {
            return other.Coordinate == Coordinate && other.Plane.W == Plane.W;
        }
    }
}
