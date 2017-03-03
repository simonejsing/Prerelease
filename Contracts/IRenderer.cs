using VectorMath;
using IReadonlyVector = VectorMath.IReadonlyVector;

namespace Contracts
{
    public interface IRenderer
    {
        IReadonlyVector GetViewport();
        void Clear(Color color);
        void DrawVector(IReadonlyVector origin, IReadonlyVector vector, Color color, float thickness = 1.0f);
        void RenderLine(IReadonlyVector point1, IReadonlyVector point2, Color color, float thickness);
        void DrawLine(IReadonlyVector point, float length, float angle, Color color, float thickness);
        void RenderPixel(IReadonlyVector position, Color color);
        void RenderRectangle(IReadonlyVector position, IReadonlyVector size, Color color);
        void RenderOpagueSprite(ISprite sprite, IReadonlyVector position, IReadonlyVector size);
        void RenderText(IFont font, IReadonlyVector position, string text, Color color);
        void RenderText(IFont font, IReadonlyVector position, string text, Color color, float rotation, IReadonlyVector origin, IReadonlyVector scale, float layerDepth = 0f);
        IRenderScope ActivateScope(string scopeName);
        void DeactivateScope(string scopeName);
    }
}