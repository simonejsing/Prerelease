using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
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

        public Projectile(ActionQueue actionQueue, Object shooter, Vector2 startingPosition, Vector2 initialVelocity, Vector2 size) : base(actionQueue, startingPosition, size)
        {
            this.Velocity = initialVelocity;
            this.Shooter = shooter;
            this.Collision += Expire;
        }

        private void Expire(object sender, ICollidableObject target, Vector2 deltaposition)
        {
            Expire();
        }

        public void Expire()
        {
            lifetime = 0;
        }
    }
}