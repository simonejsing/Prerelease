using VectorMath;

namespace Contracts
{
    public interface IGpuTexture
    {
        Vector2 Size { get; }
        bool ContentLost { get; }
    }
}