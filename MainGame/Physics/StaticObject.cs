using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class StaticObject : Object, ICollidableObject
    {
        public StaticObject(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPosition, size)
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
