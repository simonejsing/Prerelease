using System;

namespace VectorMath
{
    public struct Vector2 : IReadonlyVector, IComparable<Vector2>, IEquatable<Vector2>
    {
        public const float VectorLengthPrecission = 0.001f;

        public float X { get; }
        public float Y { get; }

        public float Length => (float) Math.Sqrt(X*X + Y*Y);
        public float LengthSquared => X * X + Y * Y;
        public float Angle => (float) Math.Atan2(Y, X);
        public bool TooSmall => LengthSquared < VectorLengthPrecission;

        public Vector2 Flip => new Vector2(-X, -Y);
        public Vector2 FlipX => new Vector2(-X, Y);
        public Vector2 FlipY => new Vector2(X, -Y);

        public Vector2(float x = 0, float y = 0)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2(IReadonlyVector v)
        {
            this.X = v.X;
            this.Y = v.Y;
        }

        public Vector2 ReplaceX(float newX)
        {
            return new Vector2(newX, Y);
        }

        public Vector2 ReplaceY(float newY)
        {
            return new Vector2(X, newY);
        }

        public Vector2 Normalize()
        {
            var len = Length;
            return new Vector2(X / len, Y / len);
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

        // Operators
        public static Vector2 operator +(Vector2 left, IReadonlyVector right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator -(Vector2 left, IReadonlyVector right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator -(Vector2 left)
        {
            return new Vector2(-left.X, -left.Y);
        }

        public static Vector2 operator *(Vector2 left, IReadonlyVector factor)
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

        public static bool operator ==(Vector2 left, IReadonlyVector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, IReadonlyVector right)
        {
            return !left.Equals(right);
        }

        // Static properties and methods
        public static Vector2 Zero = new Vector2();
        public static Vector2 Ones = new Vector2(1, 1);

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X*b.X + a.Y*b.Y;
        }

        public Vector2 Rotate(float radians)
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

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public bool Equals(Vector2 obj)
        {
            return CompareTo(obj) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Vector2 && Equals((Vector2)obj);
        }

        public override string ToString()
        {
            return string.Format("X: {0}; Y: {1}", X, Y);
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
