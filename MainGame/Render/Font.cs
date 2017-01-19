using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.Xna.Framework.Graphics;

namespace Prerelease.Main.Render
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
