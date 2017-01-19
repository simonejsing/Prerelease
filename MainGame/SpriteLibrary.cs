using System.Collections.Generic;
using Contracts;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Prerelease.Main.Render;

namespace Prerelease.Main
{
    class SpriteLibrary
    {
        private readonly ContentManager manager;
        private readonly Dictionary<string, ISprite> sprites = new Dictionary<string, ISprite>(); 

        public SpriteLibrary(ContentManager manager)
        {
            this.manager = manager;
        }

        public ISprite Load(string name)
        {
            ISprite sprite = null;
            if (sprites.TryGetValue(name, out sprite))
            {
                return sprite;
            }

            sprite = new Sprite(manager.Load<Texture2D>(@"Sprites\" + name));
            sprites.Add(name, sprite);
            return sprite;
        }
    }
}
