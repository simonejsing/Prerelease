using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.Xna.Framework.Graphics;

namespace Prerelease.Main.Render
{
    class Sprite : ISprite
    {
        public Texture2D Texture { get; private set; }

        public Sprite(Texture2D texture)
        {
            this.Texture = texture;
        }
    }
}
