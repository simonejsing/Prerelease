using System;
using System.Collections.Generic;
using System.Text;

namespace CraftingGame
{
    public class View
    {
        public ViewportProjection Viewport { get; }
        public Camera Camera { get; }

        public View(ViewportProjection viewport, Camera camera)
        {
            Viewport = viewport;
            Camera = camera;
        }
    }
}
