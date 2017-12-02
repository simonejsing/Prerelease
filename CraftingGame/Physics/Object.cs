using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class Object : IRenderableObject
    {
        private readonly ActionQueue actionQueue;

        public Rect2 BoundingBox { get; }

        public Vector2 Position
        {
            get { return BoundingBox.TopLeft; }
            set { BoundingBox.TopLeft = value; }
        }
        public UnitVector2 Facing { get; set; }

        public Vector2 Size
        {
            get { return BoundingBox.Size; }
            set { BoundingBox.Size = value; }
        }

        public IBinding<ISprite> SpriteBinding { get; set; }
        public GameAction Action { get; set; }

        public Vector2 Center => Position + 0.5f * Size;

        public Object(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size)
        {
            this.actionQueue = actionQueue;
            this.BoundingBox = new Rect2(startingPosition, size);
            this.Facing = UnitVector2.GetInstance(1, 0);
            this.Action = new GameAction(ActionType.Noop);
        }

        public void Activate(Object activator)
        {
            actionQueue.Enqueue(Action);
        }
    }
}