using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class EnemyObject : MovableObject
    {
        public int HitPoints { get; set; }
        public bool Dead => HitPoints <= 0;

        public EnemyObject(ActionQueue actionQueue) : base(actionQueue)
        {
            HitPoints = 10;
            Hit += EnemyOnHit;
            ObjectCollision += ObjectCollisionWithEnemy;
            GridCollision += OnGridCollision;
        }

        private void OnGridCollision(object sender, ICollidableObject[] target, Collision collision)
        {
            var obj = sender as EnemyObject;
            // Prevent enemies from walking over the edge
            if (obj != null && collision.VerticalCollision)
            {
                if (obj.Velocity.X < 0 && !target[6].Occupied)
                {
                    obj.Facing = new Vector2(1, 0);
                    return;
                }

                if (obj.Velocity.X > 0 && !target[8].Occupied)
                {
                    obj.Facing = new Vector2(-1, 0);
                    return;
                }
            }
            ObjectCollisionWithEnemy(sender, target[4], collision);
        }

        private void ObjectCollisionWithEnemy(object sender, ICollidableObject target, Collision collision)
        {
            var p = target as PlayerObject;
            if (p != null)
            {
                p.HitPoints = 0;
            }

            // When bumping into anything horizontally, switch direction
            var e = sender as EnemyObject;
            if (e != null && collision.HorizontalCollision)
            {
                e.Facing *= new Vector2(-1, 1);
            }
        }

        private void EnemyOnHit(object sender, IProjectile target)
        {
            var e = sender as EnemyObject;
            if (e != null)
            {
                e.HitPoints -= 1;
            }
        }
    }
}