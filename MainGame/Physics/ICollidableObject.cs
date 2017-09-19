using VectorMath;

namespace Prerelease.Main.Physics
{
    public delegate void CollisionEventHandler(object sender, ICollidableObject target);

    public interface ICollidableObject
    {
        event CollisionEventHandler Collision;

        Vector2 Position { get; }
        Vector2 Size { get; }
        Vector2 Center { get; }
        bool Occupied { get; }
    }
}