using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class Projectile : MovableObject, IProjectile
    {
        public Color Color { get; }
        public Object Shooter { get; }
        public bool Expired => lifetime <= 0;

        private int lifetime = 200;

        public void DecreaseLifespan()
        {
            lifetime--;
        }

        public Projectile(ActionQueue actionQueue, Object shooter, Color color, Vector2 startingPosition, Vector2 initialVelocity, Vector2 size) : base(actionQueue, startingPosition, size)
        {
            this.Velocity = initialVelocity;
            this.Shooter = shooter;
            this.Color = color;
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