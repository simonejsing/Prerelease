using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class Object : IRenderableObject
    {
        public ActionQueue ActionQueue { get; }

        public Rect2 BoundingBox { get; private set; }

        public Vector2 Position
        {
            get { return BoundingBox.TopLeft; }
            set { BoundingBox = new Rect2(value, BoundingBox.Size); }
        }
        public Vector2 Facing { get; set; }

        public Vector2 Size
        {
            get { return BoundingBox.Size; }
            set { BoundingBox = new Rect2(BoundingBox.TopLeft, value); }
        }

        public IBinding<ISprite> SpriteBinding { get; set; }
        public GameAction Action { get; set; }

        public Vector2 Center => Position + 0.5f * Size;

        public Object(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size)
        {
            this.ActionQueue = actionQueue;
            this.BoundingBox = new Rect2(startingPosition, size);
            this.Facing = new Vector2(1, 0);
            this.Action = new GameAction(ActionType.Noop);
        }

        public void Activate(Object activator)
        {
            ActionQueue.Enqueue(Action);
        }
    }
}