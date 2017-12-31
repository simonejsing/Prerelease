using VectorMath;

namespace Contracts
{
    public interface IRenderableObject
    {
        Color Color { get; }
        Vector2 Position { get; }
        Vector2 Facing { get; }
        Vector2 Size { get; }
        IBinding<ISprite> SpriteBinding { get; }
    }
}