using Contracts;
using CraftingGame.State;
using Serialization;
using System.Collections.Generic;
using VectorMath;

namespace CraftingGame.Physics
{
    public class StaticObject : Object, ICollidableObject
    {
        public StaticObject(ActionQueue actionQueue) : base(actionQueue)
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

        protected override void Load(StatefulObject state)
        {
            base.Load(state);
        }

        public override IDictionary<string, object> ExtractState()
        {
            return ConcatenateState(base.ExtractState(), GetState());
        }

        private IDictionary<string, object> GetState()
        {
            return new Dictionary<string, object>
            {
            };
        }
    }
}
