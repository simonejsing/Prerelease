namespace Contracts
{
    public interface IRenderScope
    {
        IFont LoadFont(string fontName);
        IBinding<ISprite> ResolveSprite(IBinding<ISprite> spriteBinding);
    }
}