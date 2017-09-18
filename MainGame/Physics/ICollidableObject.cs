using VectorMath;

namespace Prerelease.Main.Physics
{
    public interface ICollidableObject
    {
        Vector2 Position { get; }
        Vector2 Size { get; }
        Vector2 Center { get; }
        bool Occupied { get; }
    }
}