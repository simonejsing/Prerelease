using Contracts;
using VectorMath;
using Object = Prerelease.Main.Physics.Object;

namespace Prerelease.Main.Physics
{
    public class MovableObject : Object
    {
        public bool CanAccelerate { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Velocity { get; set; }
        public UnitVector2 Facing { get; set; }

        public MovableObject(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size) : base(actionQueue, startingPosition, size)
        {
            CanAccelerate = false;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            Facing = UnitVector2.GetInstance(1, 0);
        }
    }
}
