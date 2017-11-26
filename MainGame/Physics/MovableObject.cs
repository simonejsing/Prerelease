using Contracts;
using VectorMath;
using Object = Prerelease.Main.Physics.Object;

namespace Prerelease.Main.Physics
{
    public class MovableObject : Object, ICollidableObject
    {
        public event ObjectCollisionEventHandler ObjectCollision;
        public event GridCollisionEventHandler GridCollision;
        public event HitEventHandler Hit;

        public bool Grounded { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 DeltaPosition { get; set; }
        public UnitVector2 Facing { get; set; }
        public bool Occupied => true;
        //public bool Stationary

        public void OnObjectCollision(ICollidableObject target, Collision collision)
        {
            ObjectCollision?.Invoke(this, target, collision);
        }

        public void OnGridCollision(ICollidableObject[] target, Collision collision)
        {
            GridCollision?.Invoke(this, target, collision);
        }

        public void OnHit(IProjectile target)
        {
            Hit?.Invoke(this, target);
        }

        public MovableObject(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPosition, size)
        {
            Grounded = false;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            DeltaPosition = Vector2.Zero;
            Facing = UnitVector2.GetInstance(1, 0);

            ObjectCollision += HandleGroundObjectCollision;
            GridCollision += (sender, target, collision) => HandleGroundObjectCollision(sender, target[4], collision);
        }

        private static void HandleGroundObjectCollision(object sender, ICollidableObject target, Collision collision)
        {
            var obj = sender as MovableObject;

            // If a vertical collision is detected going downwards, the player has "landed" and he may accelerate
            if (obj != null && collision.VerticalCollision && obj.Velocity.Y > 0)
            {
                obj.Grounded = true;
            }
        }
    }
}
