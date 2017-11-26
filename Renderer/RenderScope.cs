using Contracts;

namespace Renderer
{
    class RenderScope : IRenderScope
    {
        private readonly FontLibrary fonts;
        private readonly SpriteLibrary sprites;

        public RenderScope(FontLibrary fonts, SpriteLibrary sprites)
        {
            this.fonts = fonts;
            this.sprites = sprites;
        }

        public IFont LoadFont(string fontName)
        {
            return fonts.LoadFont(fontName);
        }

        public IBinding<ISprite> ResolveSprite(IBinding<ISprite> spriteBinding)
        {
            return new ResolvedBinding<ISprite>(spriteBinding, LoadSprite(spriteBinding.Path));
        }

        private ISprite LoadSprite(string spriteName)
        {
            return sprites.Load(spriteName);
        }
    }
}
