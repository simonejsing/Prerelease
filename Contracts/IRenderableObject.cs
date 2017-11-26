using VectorMath;

namespace Contracts
{
    public interface IRenderableObject
    {
        Vector2 Position { get; }
        UnitVector2 Facing { get; }
        Vector2 Size { get; }
        IBinding<ISprite> SpriteBinding { get; }
    }
}