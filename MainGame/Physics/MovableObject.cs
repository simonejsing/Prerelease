using Contracts;
using VectorMath;
using Object = Prerelease.Main.Physics.Object;

namespace Prerelease.Main.Physics
{
    public class MovableObject : Object, ICollidableObject
    {
        public event CollisionEventHandler Collision;
        public event HitEventHandler Hit;

        public bool Grounded { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 DeltaPosition { get; set; }
        public UnitVector2 Facing { get; set; }
        public bool Occupied => true;
        //public bool Stationary

        public void OnCollision(ICollidableObject target, Vector2 deltaPosition)
        {
            Collision?.Invoke(this, target, deltaPosition);
        }

        public void OnHit(IProjectile target)
        {
            Hit?.Invoke(this, target);
        }

        public MovableObject(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size) : base(actionQueue, startingPosition, size)
        {
            Grounded = false;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            DeltaPosition = Vector2.Zero;
            Facing = UnitVector2.GetInstance(1, 0);
        }
    }
}
