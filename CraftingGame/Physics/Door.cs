using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public enum DestinationType { Exit, Level }

    public struct Destination
    {
        public DestinationType Type;
        public string Identifier;
    }

    public class Door : Object
    {
        public Door(ActionQueue actionQueue) : base(actionQueue)
        {
        }

        public Destination Destination { get; set; }
    }
}
