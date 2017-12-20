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
        public Door(ActionQueue actionQueue, Plane startingPlane, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPlane, startingPosition, size)
        {
        }

        public Destination Destination { get; set; }
    }
}
