using Contracts;
using VectorMath;
using Object = Prerelease.Main.Physics.Object;

namespace Prerelease.Main.Physics
{
    public class MovableObject : Object, ICollidableObject
    {
        public event CollisionEventHandler Collision;

        public bool OnGround { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 DeltaPosition { get; set; }
        public UnitVector2 Facing { get; set; }
        public bool Occupied => true;

        public void OnCollision(ICollidableObject target, Vector2 deltaPosition)
        {
            Collision?.Invoke(this, target, deltaPosition);
        }

        public MovableObject(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size) : base(actionQueue, startingPosition, size)
        {
            OnGround = false;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            DeltaPosition = Vector2.Zero;
            Facing = UnitVector2.GetInstance(1, 0);
        }
    }
}
