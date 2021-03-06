﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public struct Rect2
    {
        public Vector2 TopLeft { get; set; }
        public Vector2 TopRight => new Vector2(TopLeft.X + Size.X, TopLeft.Y);
        public Vector2 BottomLeft => new Vector2(TopLeft.X, TopLeft.Y - Size.Y);
        public Vector2 BottomRight => new Vector2(TopLeft.X + Size.X, TopLeft.Y - Size.Y);
        public Vector2 Size { get; set; }

        public Rect2(IReadonlyVector position, IReadonlyVector size)
        {
            TopLeft = new Vector2(position);
            Size = new Vector2(size);
        }

        public bool Intersects(IReadonlyVector point)
        {
            var br = BottomRight;
            return TopLeft.X <= point.X && br.X >= point.X && br.Y <= point.Y && TopLeft.Y >= point.Y;
        }

        public bool Intersects(Rect2 rect)
        {
            return 
                (
                    Intersects(rect.TopLeft) ||
                    Intersects(rect.TopRight) ||
                    Intersects(rect.BottomLeft) ||
                    Intersects(rect.BottomRight)
                )
                ||
                (
                    rect.Intersects(TopLeft) ||
                    rect.Intersects(TopRight) ||
                    rect.Intersects(BottomLeft) ||
                    rect.Intersects(BottomRight)
                );
        }
    }
}
