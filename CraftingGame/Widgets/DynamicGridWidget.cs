using System;
using Contracts;
using VectorMath;

namespace CraftingGame.Widgets
{
    internal class DynamicGridWidget
    {
        private readonly IRenderer renderer;
        private readonly IFont font;
        private readonly int blockSize;

        public DynamicGridWidget(IRenderer renderer, IFont font, int blockSize)
        {
            this.renderer = renderer;
            this.font = font;
            this.blockSize = blockSize;
        }

        public void Render(ViewportProjection view)
        {
            var gridSize = blockSize * 10;
            var activeView = view.Projection;

            var gridTop = (int)Math.Floor(activeView.TopLeft.Y / gridSize);
            var gridBottom = (int)Math.Floor(activeView.BottomLeft.Y / gridSize);

            //var gridBottom = (int)Math.Floor(View.MapToWorld(Vector2.Zero).Y / gridSize);
            //var gridTop = (int)Math.Floor(View.MapSizeToWorld(View.ViewPort).Y / gridSize);

            for (var y = gridBottom; y <= gridTop; y += 1)
            {
                var c = y >= 0 ? Color.Blue : Color.Red;
                if (y == 0)
                    c = Color.DarkGray;
                var p = view.MapToViewport(new Vector2(0, y * gridSize));
                renderer.RenderVector(new Vector2(0, p.Y), new Vector2(view.DisplaySize.X, 0), c, 3);

                // Render y-labels
                renderer.RenderText(
                    font,
                    view.MapToViewport(new Vector2(0, y * gridSize)),
                    $"(0,{y * gridSize})",
                    c);
            }

            var gridLeft = (int)Math.Floor(activeView.TopLeft.X / gridSize);
            var gridRight = (int)Math.Floor(activeView.TopRight.X / gridSize);
            for (var x = gridLeft; x <= gridRight; x += 1)
            {
                var c = x >= 0 ? Color.Blue : Color.Red;
                if (x == 0)
                    c = Color.DarkGray;
                var p = view.MapToViewport(new Vector2(x * gridSize, 0));
                renderer.RenderVector(new Vector2(p.X, 0), new Vector2(0, -view.DisplaySize.Y), c, 3);

                // Render x-labels
                renderer.RenderText(
                    font,
                    view.MapToViewport(new Vector2(x * gridSize, 0)),
                    $"({x * gridSize},0)",
                    c);
            }
        }
    }
}
