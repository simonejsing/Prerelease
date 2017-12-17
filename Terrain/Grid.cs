using System;
using VectorMath;

namespace Terrain
{
    public class Grid
    {
        public Vector2 Size { get; }

        public Grid(float sizeX, float sizeY)
        {
            Size = new Vector2(sizeX, sizeY);
        }

        public Grid(Vector2 size) : this(size.X, size.Y)
        {
        }

        public Vector2 GridCoordinateToPoint(Coordinate c)
        {
            return GridCoordinateToPoint(c.U, c.V);
        }

        public Vector2 GridCoordinateToPoint(int u, int v)
        {
            return new Vector2(u * Size.X, v * Size.Y);
        }

        public Coordinate PointToGridCoordinate(Vector2 v)
        {
            return PointToGridCoordinate(v.X, v.Y);
        }

        public Coordinate PointToGridCoordinate(float x, float y)
        {
            return new Coordinate()
            {
                U = (int)Math.Floor(x / Size.X),
                V = (int)Math.Floor(y / Size.Y),
            };
        }
    }
}
