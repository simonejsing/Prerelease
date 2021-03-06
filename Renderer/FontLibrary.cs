﻿using System.Collections.Generic;
using Contracts;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer
{
    class FontLibrary
    {
        private readonly Dictionary<string, IFont> loadedFonts = new Dictionary<string, IFont>(); 
        private readonly ContentManager manager;

        public FontLibrary(ContentManager manager)
        {
            this.manager = manager;
        }

        public IFont LoadFont(string fontName)
        {
            IFont font = null;
            if (loadedFonts.TryGetValue(fontName, out font))
            {
                return font;
            }

            font = new Font(manager.Load<SpriteFont>(@"Fonts\" + fontName));
            loadedFonts.Add(fontName, font);
            return font;
        }
    }
}
