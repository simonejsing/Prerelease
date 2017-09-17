using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    internal class Projectile : MovableObject
    {
        public Object Shooter { get; }
        public bool Expired => lifetime <= 0;

        private int lifetime = 200;

        public void Update(float timestep)
        {
            Position += Velocity*timestep;
            lifetime--;
        }

        public Projectile(ActionQueue actionQueue, Object shooter, Vector2 startingPosition, Vector2 initialVelocity, Vector2 size) : base(actionQueue, startingPosition, size)
        {
            this.Velocity = initialVelocity;
            this.Shooter = shooter;
        }
    }
}