using Contracts;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer
{
    class Font : IFont
    {
        public SpriteFont SpriteFont { get; private set; }

        public Font(SpriteFont font)
        {
            SpriteFont = font;
        }
    }
}
