using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public class Polygon2
    {
        public Vector2[] PointsArray { get; private set; }

        public IEnumerable<Vector2> Points
        {
            get
            {
                return PointsArray;
            }
            set
            {
                PointsArray = value.ToArray();
            }
        }

        public IEnumerable<PointVector2> Sides
        {
            get
            {
                for (var i = 0; i < PointsArray.Length; i++)
                {
                    var from = PointsArray[i];
                    var to = PointsArray[(i + 1) % PointsArray.Length];
                    yield return new PointVector2(from, to - from);
                }
            }
        }

        public Polygon2(params Vector2[] points)
        {
            Points = points;
        }

        public bool Intersect(Vector2 point)
        {
            return pnpoly(PointsArray.Length, PointsArray, point.X, point.Y);
        }

        public bool Intersect(PointVector2 vector)
        {
            // Check intersection with all lines
            if (Sides.Any(side => side.Intersect(vector)))
            {
                return true;
            }

            // Check if either point is inside the polygin
            return Intersect(vector.Origin) || Intersect(vector.Origin + vector.Vector);
        }

        bool pnpoly(int nvert, Vector2[] vert, float testx, float testy)
        {
            int i, j;
            var c = false;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((vert[i].Y > testy) != (vert[j].Y > testy)) &&
                 (testx < (vert[j].X - vert[i].X) * (testy - vert[i].Y) / (vert[j].Y - vert[i].Y) + vert[i].X))
                    c = !c;
            }
            return c;
        }

    }
}
