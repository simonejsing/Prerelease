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
        public Door(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPosition, size)
        {
        }

        public Destination Destination { get; set; }
    }
}
