using VectorMath;

namespace Prerelease.Main.Render
{
    class Transformation
    {
        private Matrix2x2 matrix;

        public Transformation()
        {
            this.matrix = Matrix2x2.Identity();
        }

        public void Scale(float scaleX, float scaleY)
        {
            this.matrix *= new Matrix2x2(scaleX, 0f, 0f, scaleY);
        }

        public Vector2 Apply(Vector2 vector)
        {
            return matrix*vector;
        }
    }
}
