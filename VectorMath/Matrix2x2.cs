using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public class Matrix2x2
    {
        public Vector2[] Columns { get; }

        public float X1 => Columns[0].X;
        public float X2 => Columns[1].X;
        public float Y1 => Columns[0].Y;
        public float Y2 => Columns[1].Y;

        public Matrix2x2(float x1, float y1, float x2, float y2)
        {
            Columns = new Vector2[2];

            Columns[0] = new Vector2(x1, y1);
            Columns[1] = new Vector2(x2, y2);
        }

        public Matrix2x2(Vector2 firstColumn, Vector2 secondColumn)
        {
            Columns = new Vector2[2];

            Columns[0] = new Vector2(firstColumn);
            Columns[1] = new Vector2(secondColumn);
        }

        public static Matrix2x2 Identity()
        {
            return new Matrix2x2(new Vector2(1, 0), new Vector2(0, 1));
        }

        public static Vector2 operator *(Matrix2x2 matrix, Vector2 vector)
        {
            return new Vector2(
                matrix.X1 * vector.X + matrix.X2 * vector.Y,
                matrix.Y1 * vector.X + matrix.Y2 * vector.Y);
        }

        public static Matrix2x2 operator *(Matrix2x2 left, Matrix2x2 right)
        {
            // c1 c2
            // x1 x2
            // y1 y2
            //

            return new Matrix2x2(
                    left.X1 * right.X1 + left.X2 * right.Y1,
                    left.Y1 * right.X1 + left.Y2 * right.Y1,
                    left.X1 * right.X2 + left.X2 * right.Y2,
                    left.Y1 * right.X2 + left.Y2 * right.Y2
                );
        }
    }
}
