using System;
using VectorMath;

namespace Contracts
{
    public interface IRenderer
    {

        void ResetTransform();
        void Scale(float x, float y);

        IReadonlyVector GetViewport();
        void Clear(Color color);
        void RenderVector(IReadonlyVector origin, IReadonlyVector vector, Color color, float thickness = 1.0f);
        void RenderLine(IReadonlyVector point1, IReadonlyVector point2, Color color, float thickness);
        void DrawLine(IReadonlyVector point, float length, float angle, Color color, float thickness);
        void RenderPixel(IReadonlyVector position, Color color);
        void RenderRectangle(IReadonlyVector position, IReadonlyVector size, Color color);
        void RenderOpagueSprite(IGpuTexture texture, IReadonlyVector position, IReadonlyVector size, bool flipHorizontally = false);
        void RenderOpagueSprite(ISprite sprite, IReadonlyVector position, IReadonlyVector size, bool flipHorizontally = false);
        void RenderText(IFont font, IReadonlyVector position, string text, Color color);
        void RenderText(IFont font, IReadonlyVector position, string text, Color color, float rotation, IReadonlyVector origin, IReadonlyVector scale, float layerDepth = 0f);
        IGpuTexture InitializeGpuTexture(int width, int height);
        void RenderToGpuTexture(IGpuTexture sprite, Action renderAction);
        ISprite CreateTexture(int width, int height, Action renderAction);
        IRenderScope ActivateScope(string scopeName);
        void DeactivateScope(string scopeName);
    }
}