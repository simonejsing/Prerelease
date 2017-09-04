using Contracts;
using VectorMath;

namespace HubSceene
{
    public class SceeneObject
    {
        private readonly ActionQueue actionQueue;

        public ISprite Sprite { get; set; }
        public GameAction Action { get; set; }

        public SceeneObject(ActionQueue actionQueue)
        {
            this.actionQueue = actionQueue;
            this.Action = new GameAction(ActionType.Noop);
        }

        public void Activate(SceeneObject activator)
        {
            actionQueue.Enqueue(Action);
        }
    }
}