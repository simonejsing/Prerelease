using VectorMath;

namespace Prerelease.Main.Physics
{
    public interface ICollidableObject
    {
        IReadonlyVector Position { get; }
        bool Occupied { get; }
    }
}