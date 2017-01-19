namespace Contracts
{
    public interface IRenderScope
    {
        IFont LoadFont(string fontName);
        ISprite LoadSprite(string spriteName);
    }
}