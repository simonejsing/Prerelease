using VectorMath;

namespace Contracts
{
    public interface IRenderableObject
    {
        Vector2 Position { get; }
        Vector2 Facing { get; }
        Vector2 Size { get; }
        IBinding<ISprite> SpriteBinding { get; }
    }
}