using System;
using VectorMath;

namespace CraftingGame
{
    // The view is a projection of the view port into the game world, it contains a scale factor and a translation
    public class ViewportProjection
    {
        private Vector2 viewPort;
        private Matrix2x2 viewTransform = Matrix2x2.Identity;
        private Matrix2x2 viewInverseTransform = Matrix2x2.Identity;
        private Vector2 viewTranslation = Vector2.Zero;

        // The projection of the viewport into the game world
        public Rect2 Projection { get; private set; }

        public ViewportProjection(IReadonlyVector viewPort)
        {
            this.viewPort = new Vector2(viewPort);
            UpdateProjection();
        }

        public void Translate(IReadonlyVector translate)
        {
            viewTranslation += translate;
            UpdateProjection();
        }

        public void Center(IReadonlyVector point)
        {
            viewTranslation = -0.5f * viewPort.FlipY + point;
            UpdateProjection();
        }

        public void Scale(float scale)
        {
            Scale(new Vector2(scale, scale));
        }

        public void Scale(IReadonlyVector scale)
        {
            viewTransform = new Matrix2x2(scale.X, 0, 0, scale.Y);
            viewInverseTransform = new Matrix2x2(1f / scale.X, 0, 0, 1f / scale.Y);
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            Projection = new Rect2(MapToWorld(Vector2.Zero), MapSizeToWorld(viewPort));
        }

        public IReadonlyVector MapToWorld(IReadonlyVector viewPortPixel)
        {
            return viewTranslation + viewTransform * viewPortPixel;
        }

        public IReadonlyVector MapSizeToWorld(IReadonlyVector size)
        {
            return viewTransform * size;
        }

        public IReadonlyVector MapToViewport(IReadonlyVector point)
        {
            return (-viewTranslation) + viewInverseTransform * point;
        }

        public IReadonlyVector MapSizeToViewport(IReadonlyVector size)
        {
            return viewInverseTransform * size;
        }
    }
}