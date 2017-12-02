using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class StaticObject : Object, ICollidableObject, ICollectableObject
    {
        public StaticObject(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPosition, size)
        {
        }

        public event ObjectCollisionEventHandler ObjectCollision;
        public event GridCollisionEventHandler GridCollision;
        public event HitEventHandler Hit;
        public event CollectEventHandler Collect;

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

        public void OnCollect(ICollectingObject target)
        {
            Collect?.Invoke(this, target);
        }

        public bool Occupied => true;
        public bool PickedUp { get; set; }
    }
}
