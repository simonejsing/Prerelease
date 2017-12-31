using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class Projectile : MovableObject, IProjectile
    {
        public Object Shooter { get; }
        public bool Expired => lifetime <= 0;

        private int lifetime = 200;

        public void DecreaseLifespan()
        {
            lifetime--;
        }

        public Projectile(ActionQueue actionQueue, Object shooter) : base(actionQueue)
        {
            this.Shooter = shooter;
            this.ObjectCollision += ProjectileObjectCollision;
        }

        private void ProjectileObjectCollision(object sender, ICollidableObject target, Collision collision)
        {
            Expire();
        }

        public void Expire()
        {
            lifetime = 0;
        }
    }
}