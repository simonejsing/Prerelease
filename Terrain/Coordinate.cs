namespace Terrain
{
    public struct Coordinate
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
    }
}