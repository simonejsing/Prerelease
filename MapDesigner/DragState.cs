using Microsoft.Xna.Framework.Input;
using VectorMath;

namespace MapDesigner
{
    public class DragState
    {
        private bool isDragging = false;
        private Vector2 dragStartPoint = Vector2.Zero;
        private Vector2 dragPoint = Vector2.Zero;
        private Vector2 accumulatedDragOffset = Vector2.Zero;

        public Vector2 Offset => accumulatedDragOffset + (isDragging ? (dragStartPoint - dragPoint) : Vector2.Zero);

        public void Update(MouseState mouseInput)
        {
            if (isDragging)
            {
                dragPoint = new Vector2(mouseInput.X, mouseInput.Y);
                if (mouseInput.LeftButton == ButtonState.Released)
                {
                    isDragging = false;
                    accumulatedDragOffset += (dragStartPoint - dragPoint);
                }
            }
            else
            {
                if (mouseInput.LeftButton == ButtonState.Pressed)
                {
                    isDragging = true;
                    dragStartPoint = new Vector2(mouseInput.X, mouseInput.Y);
                    dragPoint = new Vector2(mouseInput.X, mouseInput.Y);
                }
            }
        }
    }
}