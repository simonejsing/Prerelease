using VectorMath;
using Point = Microsoft.Xna.Framework.Point;

namespace Renderer
{
    class PointAdaptor : IReadonlyPoint
    {
        private Point point;

        public int X => point.X;
        public int Y => point.Y;

        public PointAdaptor(Point p)
        {
            this.point = p;
        }
    }
}
