using System;
using VectorMath;

namespace CraftingGame
{
    // The view is a projection of the view port into the game world, it contains a scale factor and a translation
    public class ViewportProjection
    {
        private Matrix2x2 viewTransform = Matrix2x2.Identity;
        private Matrix2x2 viewInverseTransform = Matrix2x2.Identity;
        private Vector2 viewTranslation = Vector2.Zero;

        public Vector2 DisplaySize { get; }
        public Vector2 Origin { get; }
        // The projection of the viewport into the game world
        public Rect2 Projection { get; private set; }

        public ViewportProjection(IReadonlyVector displaySize) : this(displaySize, Vector2.Zero)
        {
        }

        public ViewportProjection(IReadonlyVector displaySize, IReadonlyVector origin)
        {
            this.DisplaySize = new Vector2(displaySize);
            this.Origin = new Vector2(origin);
            UpdateProjection();
        }

        public static ViewportProjection ToTexture(Vector2 textureSize)
        {
            var projection = new ViewportProjection(textureSize, Matrix2x2.ProjectY * textureSize);
            projection.Scale(new Vector2(1, -1));
            return projection;
        }

        public void Translate(IReadonlyVector translate)
        {
            viewTranslation += translate;
            UpdateProjection();
        }

        public void Center(IReadonlyVector point)
        {
            viewTranslation = -0.5f * (viewTransform*(DisplaySize - Origin)).FlipY + point;
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
            viewTranslation = viewTransform*viewTranslation;
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            Projection = new Rect2(MapToWorld(-Origin), MapSizeToWorld(DisplaySize));
        }

        public Vector2 MapToWorld(IReadonlyVector viewPortPixel)
        {
            return Origin + viewTranslation + viewTransform * (viewPortPixel);
        }

        public Vector2 MapSizeToWorld(IReadonlyVector size)
        {
            return viewTransform * size;
        }

        public Vector2 MapToViewport(IReadonlyVector point)
        {
            return viewInverseTransform * ((-viewTranslation) + (-Origin + point));
        }

        public Vector2 MapSizeToViewport(IReadonlyVector size)
        {
            return viewInverseTransform * size;
        }

        public ViewportProjection[] SplitVertically()
        {
            var leftViewport = new ViewportProjection(new Vector2(DisplaySize.X / 2, DisplaySize.Y));
            var rightViewport = new ViewportProjection(new Vector2(DisplaySize.X / 2, DisplaySize.Y), new Vector2(-DisplaySize.X / 2, 0));
            return new ViewportProjection[]
            {
                leftViewport,
                rightViewport,
            };
        }
    }
}