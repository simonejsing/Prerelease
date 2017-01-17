using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public class PointVector2
    {
        public Vector2 Origin { get; set; }
        public Vector2 Vector { get; set; }

        public Vector2 Destination
        {
            get
            {
                return Origin + Vector;
            }
        }

        public Line2 Line
        {
            get
            {
                return new Line2(Origin, Vector.Hat()); 
            }
        }

        public PointVector2 Reverse
        {
            get
            {
                return new PointVector2(Origin + Vector, -Vector);
            }
        }

        public PointVector2(Vector2 origin, Vector2 vector)
        {
            Origin = origin;
            Vector = vector;
        }

        public PointVector2(PointVector2 pointVector)
        {
            Origin = new Vector2(pointVector.Origin);
            Vector = new Vector2(pointVector.Vector);
        }

        public bool TooSmall()
        {
            return Vector.TooSmall();
        }

        public bool Intersect(Line2 line)
        {
            return IntersectionTest(IntersectionFactor(line), Vector.LengthSquared);
        }

        public float IntersectionFactor(Line2 line)
        {
            // Get the length factor for where 'vector' intersects this vector
            var t = Line.IntersectionFactor(line);

            // If t = NaN then the vector overlaps the line, given line is infinite we intersect at every point
            if (float.IsNaN(t))
                return 0.0f;

            return t;
        }
        
        public static bool IntersectionTest(float t, float lengthSquared)
        {
            return !(t < 0.0f) && !(t*t > lengthSquared);
        }

        public bool Intersect(PointVector2 vector)
        {
            // If either vector has zero length it is impossible to intersect
            if (TooSmall() || vector.TooSmall())
                return false;

            var t1 = Line.IntersectionFactor(vector.Line);
            var t2 = vector.Line.IntersectionFactor(Line);

            // t1 = NaN is a special case where the two lines overlap
            if (float.IsNaN(t1))
            {
                // Project all four points onto same vector (this.Vector)
                var pl1 = Origin.ProjectionLength(Vector);
                var pl2 = (Origin + Vector).ProjectionLength(Vector);

                var pl3 = vector.Origin.ProjectionLength(Vector);
                var pl4 = (vector.Origin + vector.Vector).ProjectionLength(Vector);

                // Find min/max for 'this'
                var minA = pl1 < pl2 ? pl1 : pl2;
                var maxA = pl1 > pl2 ? pl1 : pl2;

                // Find min/max for vector
                var minB = pl3 < pl4 ? pl3 : pl4;
                var maxB = pl3 > pl4 ? pl3 : pl4;

                // The vectors do not overlap if either of the following conditions are true:
                //   minA > maxB
                //   minB > maxA

                return !(minA > maxB || minB > maxA);
            }

            return IntersectionTest(t1, Vector.LengthSquared) && IntersectionTest(t2, vector.Vector.LengthSquared);
        }

        public override string ToString()
        {
            return String.Format("{0} -> {1}", Origin, Origin + Vector);
        }

        protected bool Equals(PointVector2 other)
        {
            return Equals(Origin, other.Origin) && Equals(Vector, other.Vector);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PointVector2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Origin != null ? Origin.GetHashCode() : 0) * 397) ^ (Vector != null ? Vector.GetHashCode() : 0);
            }
        }

        public static bool operator ==(PointVector2 left, PointVector2 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PointVector2 left, PointVector2 right)
        {
            return !Equals(left, right);
        }
    }
}
