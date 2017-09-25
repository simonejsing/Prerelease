using VectorMath;

namespace Prerelease.Main.Physics
{
    public delegate void CollisionEventHandler(object sender, ICollidableObject target, Collision collision);
    public delegate void HitEventHandler(object sender, IProjectile target);

    public interface ICollidableObject
    {
        event CollisionEventHandler Collision;
        event HitEventHandler Hit;

        Vector2 Position { get; }
        Vector2 Size { get; }
        Vector2 Center { get; }
        bool Occupied { get; }
    }
}