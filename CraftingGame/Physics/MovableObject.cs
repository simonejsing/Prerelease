using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class MovableObject : StaticObject
    {
        public bool Grounded { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 DeltaPosition { get; set; }

        public MovableObject(ActionQueue actionQueue, Plane startingPlane, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPlane, startingPosition, size)
        {
            Grounded = false;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            DeltaPosition = Vector2.Zero;

            ObjectCollision += HandleGroundObjectCollision;
            GridCollision += (sender, target, collision) => HandleGroundObjectCollision(sender, target[4], collision);
        }

        private static void HandleGroundObjectCollision(object sender, ICollidableObject target, Collision collision)
        {
            var obj = sender as MovableObject;

            // If a vertical collision is detected going downwards, the player has "landed" and he may accelerate
            if (obj != null && collision.VerticalCollision && obj.Velocity.Y < 0)
            {
                obj.Grounded = true;
            }
        }
    }
}
