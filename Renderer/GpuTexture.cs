using Contracts;
using Microsoft.Xna.Framework.Graphics;
using VectorMath;

namespace Renderer
{
    internal class GpuTexture : IGpuTexture
    {
        public RenderTarget2D RenderTarget { get; }
        public bool ContentLost => RenderTarget?.IsContentLost ?? true;
        public Vector2 Size { get; }
        public IReadonlyRectangle SourceRectangle { get; private set; }

        public GpuTexture(RenderTarget2D renderTarget, Vector2 size)
        {
            this.RenderTarget = renderTarget;
            this.Size = size;
        }
    }
}