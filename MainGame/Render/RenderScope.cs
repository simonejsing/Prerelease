using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Prerelease.Main.Render
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

        public ISprite LoadSprite(string spriteName)
        {
            return sprites.Load(spriteName);
        }
    }
}
