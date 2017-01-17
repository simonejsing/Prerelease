using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public class UnitVector2 : Vector2
    {
        public static UnitVector2 GetInstance(float x, float y)
        {
            var len = (float) Math.Sqrt(x*x + y*y);
            return new UnitVector2(x/len, y/len);
        }

        public static UnitVector2 GetInstance(Vector2 direction)
        {
            return GetInstance(direction.X, direction.Y);
        }

        protected UnitVector2(float x, float y) : base(x, y)
        {
        }

        public UnitVector2(UnitVector2 source) : base(source)
        {
        }

        public override float Length
        {
            get { return 1.0f; }
        }

        public override float LengthSquared
        {
            get { return 1.0f; }
        }

        public new UnitVector2 Hat()
        {
            return new UnitVector2(-Y, X);
        }

        public override UnitVector2 Normalize()
        {
            return new UnitVector2(this);
        }

        public new UnitVector2 Rotate(float radians)
        {
            // Recreate the unit vector to ensure the vector stays close to unit over multiple rotations
            return UnitVector2.GetInstance((this as Vector2).Rotate(radians));
        }

        public override bool TooSmall()
        {
            return false;
        }
    }
}
