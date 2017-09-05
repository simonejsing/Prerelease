using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class Object
    {
        private readonly ActionQueue actionQueue;

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public ISprite Sprite { get; set; }
        public GameAction Action { get; set; }

        public Object(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size)
        {
            this.actionQueue = actionQueue;
            this.Position = startingPosition;
            this.Size = size;
            this.Action = new GameAction(ActionType.Noop);
        }

        public void Activate(Object activator)
        {
            actionQueue.Enqueue(Action);
        }
    }
}