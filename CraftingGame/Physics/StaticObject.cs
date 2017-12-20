using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class StaticObject : Object, ICollidableObject
    {
        public StaticObject(ActionQueue actionQueue, Plane startingPlane, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPlane, startingPosition, size)
        {
        }

        public event ObjectCollisionEventHandler ObjectCollision;
        public event GridCollisionEventHandler GridCollision;
        public event HitEventHandler Hit;

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

        public bool Occupied => true;
    }
}
