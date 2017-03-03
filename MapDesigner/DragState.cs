using Microsoft.Xna.Framework.Input;
using VectorMath;

namespace MapDesigner
{
    public class DragState
    {
        private Vector2 dragStartPoint = Vector2.Zero;
        private Vector2 dragPoint = Vector2.Zero;
        private Vector2 accumulatedDragOffset = Vector2.Zero;

        public bool IsDragging { get; private set; } = false;

        public Vector2 Offset => accumulatedDragOffset + (IsDragging ? (dragStartPoint - dragPoint) : Vector2.Zero);

        public void Update(MouseState mouseInput)
        {
            if (IsDragging)
            {
                dragPoint = new Vector2(mouseInput.X, mouseInput.Y);
                if (mouseInput.RightButton == ButtonState.Released)
                {
                    IsDragging = false;
                    accumulatedDragOffset += (dragStartPoint - dragPoint);
                }
            }
            else
            {
                if (mouseInput.RightButton == ButtonState.Pressed)
                {
                    IsDragging = true;
                    dragStartPoint = new Vector2(mouseInput.X, mouseInput.Y);
                    dragPoint = new Vector2(mouseInput.X, mouseInput.Y);
                }
            }
        }
    }
}