using Microsoft.Xna.Framework;
using VectorMath;
using Point = VectorMath.Point;

namespace Renderer
{
    public class RectangleAdaptor : IReadonlyRectangle
    {
        public Rectangle Rect { get; private set; }

        public IReadonlyPoint Position => new Point(Rect.Location.X, Rect.Location.Y);
        public IReadonlyPoint Size => new Point(Rect.Size.X, Rect.Size.Y);

        public RectangleAdaptor(Rectangle rect)
        {
            this.Rect = rect;
        }
    }
}
