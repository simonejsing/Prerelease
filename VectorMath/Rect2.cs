using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public class Rect2
    {
        public Vector2 TopLeft { get; set; }
        public Vector2 Size { get; set;  }
        public Vector2 BottomRight => TopLeft + Size;

        public Rect2(IReadonlyVector position, IReadonlyVector size)
        {
            TopLeft = new Vector2(position);
            Size = new Vector2(size);
        }

        public bool Inside(IReadonlyVector point)
        {
            var br = BottomRight;
            return TopLeft.X <= point.X && br.X >= point.X && TopLeft.Y <= point.Y && br.Y >= point.Y;
        }
    }
}
