using Vector2 = VectorMath.Vector2;

namespace Contracts
{
    public interface IRenderer
    {
        Vector2 GetViewport();
        void Clear(Color color);
        void Begin();
        void End();
        void DrawVector(Vector2 origin, Vector2 vector, Color color, float thickness = 1.0f);
        void RenderLine(Vector2 point1, Vector2 point2, Color color, float thickness);
        void DrawLine(Vector2 point, float length, float angle, Color color, float thickness);
        void RenderPixel(Vector2 position, Color color);
        void RenderOpagueSprite(ISprite sprite, Vector2 position);
        void RenderText(IFont font, Vector2 position, string text, Color color);
        void RenderText(IFont font, Vector2 position, string text, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth = 0f);
        IRenderScope ActivateScope(string scopeName);
        void DeactivateScope(string scopeName);
    }
}