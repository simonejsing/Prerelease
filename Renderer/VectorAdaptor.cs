using VectorMath;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Renderer
{
    class VectorAdaptor : IReadonlyVector
    {
        private Vector2 vector;

        public float X => vector.X;
        public float Y => vector.Y;

        public VectorAdaptor(Vector2 v)
        {
            this.vector = v;
        }
    }
}
