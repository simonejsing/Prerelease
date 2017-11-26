using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class Object
    {
        private readonly ActionQueue actionQueue;

        public Rect2 BoundingBox { get; }

        public Vector2 Position
        {
            get { return BoundingBox.TopLeft; }
            set { BoundingBox.TopLeft = value; }
        }

        public Vector2 Size
        {
            get { return BoundingBox.Size; }
            set { BoundingBox.Size = value; }
        }

        public ISprite Sprite { get; set; }
        public GameAction Action { get; set; }

        public Vector2 Center => Position + 0.5f * Size;

        public Object(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size)
        {
            this.actionQueue = actionQueue;
            this.BoundingBox = new Rect2(startingPosition, size);
            this.Action = new GameAction(ActionType.Noop);
        }

        public void Activate(Object activator)
        {
            actionQueue.Enqueue(Action);
        }
    }
}