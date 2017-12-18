using System;

namespace Terrain
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        public int U;
        public int V;

        public Coordinate(int u, int v)
        {
            U = u;
            V = v;
        }

        public static Coordinate operator +(Coordinate self, Coordinate other)
        {
            return new Coordinate(self.U + other.U, self.V + other.V);
        }

        public static Coordinate operator -(Coordinate self, Coordinate other)
        {
            return new Coordinate(self.U - other.U, self.V - other.V);
        }

        public static Coordinate operator *(Coordinate self, Coordinate other)
        {
            return new Coordinate(self.U * other.U, self.V * other.V);
        }

        public static Coordinate operator /(Coordinate self, Coordinate other)
        {
            return new Coordinate(self.U / other.U, self.V / other.V);
        }

        public override string ToString()
        {
            return $"({U},{V})";
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + U;
            hash = hash * 31 + V;
            return hash;
        }

        public override bool Equals(object other)
        {
            return other is Coordinate ? Equals((Coordinate)other) : false;
        }

        public bool Equals(Coordinate other)
        {
            return other.U == U && other.V == V;
        }
    }
}