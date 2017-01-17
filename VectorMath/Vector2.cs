using System;

namespace VectorMath
{
    public class Vector2 : IComparable<Vector2>
    {
        public const float VectorLengthPrecission = 0.001f;

        public float X { get; set; }
        public float Y { get; set; }

        public virtual float Length {
            get
            {
                return (float) Math.Sqrt(X*X + Y*Y);
            }
        }

        public virtual float LengthSquared
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public float Angle
        {
            get
            {
                return (float) Math.Atan2(Y, X);
            }
        }

        public Vector2()
        {
            this.X = 0;
            this.Y = 0;
        }

        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2(Vector2 v)
        {
            this.X = v.X;
            this.Y = v.Y;
        }

        public virtual UnitVector2 Normalize()
        {
            return UnitVector2.GetInstance(this.X, this.Y);
        }

        public Vector2 Hat()
        {
            return new Vector2(-Y, X);
        }

        /// <summary>
        /// Returns a vector that is the projection of 'this' vector onto vector 'v'
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The projected vector</returns>
        public Vector2 ProjectOn(Vector2 v)
        {
            return v.Normalize()*ProjectionLength(v);
        }

        /// <summary>
        /// Computes the scaling factor 't' along the unit 'v' vector where the projection 'p' of 'this' falls such that t * unit(v) = p
        /// the resulting value is the length of 'p'
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public float ProjectionLength(Vector2 v)
        {
            return Dot(this, v) / v.Length;
        }

        public virtual bool TooSmall()
        {
            return LengthSquared < VectorLengthPrecission;
        }

        // Operators
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator -(Vector2 left)
        {
            return new Vector2(-left.X, -left.Y);
        }

        public static Vector2 operator *(Vector2 left, Vector2 factor)
        {
            return new Vector2(left.X * factor.X, left.Y * factor.Y);
        }

        public static Vector2 operator *(Vector2 left, float factor)
        {
            return new Vector2(left.X * factor, left.Y * factor);
        }

        public static Vector2 operator *(float factor, Vector2 right)
        {
            return right*factor;
        }

        public static Vector2 operator /(Vector2 left, float factor)
        {
            return new Vector2(left.X / factor, left.Y / factor);
        }

        // Static properties and methods
        public static Vector2 Zero
        {
            get
            {
                return new Vector2();
            }
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X*b.X + a.Y*b.Y;
        }

        public virtual Vector2 Rotate(float radians)
        {
            float cosAngle = (float)Math.Cos(radians);
            float sinAngle = (float)Math.Sin(radians);

            return new Vector2(X*cosAngle - Y*sinAngle, X*sinAngle + Y*cosAngle);
        }

        public static float DistanceBetween(Vector2 a, Vector2 b)
        {
            return (a - b).Length;
        }

        public static float DistanceBetweenSquared(Vector2 a, Vector2 b)
        {
            return (a - b).LengthSquared;
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            var dotProduct = Dot(a, b)/(a.Length*b.Length);
            return (float) Math.Acos(dotProduct);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            
            var v = obj as Vector2;
            return Equals(v);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public bool Equals(Vector2 obj)
        {
            if (obj == null)
                return false;

            return CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            return String.Format("X: {0}; Y: {1}", X, Y);
        }

        public int CompareTo(Vector2 other)
        {
            var compareToX = X.CompareTo(other.X);
            switch (compareToX)
            {
                case 0:
                    return Y.CompareTo(other.Y);
                default:
                    return compareToX;
            }
        }
    }
}
