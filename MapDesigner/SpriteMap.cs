using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;
using VectorMath;
using Vector2 = VectorMath.Vector2;

namespace MapDesigner
{
    class SpriteMap
    {
        private int tileWidth;
        private int tileHeight;
        private int nx;
        private int ny;

        private ISprite[,] spriteCache;

        public Texture2D Texture { get; private set; }

        public SpriteMap(Texture2D texture, int spriteTileWidth, int spriteTileHeight)
        {
            this.Texture = texture;
            this.tileWidth = spriteTileWidth;
            this.tileHeight = spriteTileHeight;

            this.nx = this.Texture.Width / this.tileWidth;
            this.ny = this.Texture.Height / this.tileHeight;
            this.spriteCache = new ISprite[nx,ny];
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    this.spriteCache[x, y] = null;
                }
            }
        }

        public Rectangle Tile(int x, int y)
        {
            return new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
        }

        public ISprite Sprite(int x, int y)
        {
            if (x >= this.nx || y >= this.ny)
            {
                throw new Exception("Invalid sprite index into sprite map.");
            }

            if (this.spriteCache[x, y] == null)
            {
                this.spriteCache[x, y] = GenerateSpriteTile(x,y);
            }

            return this.spriteCache[x, y];
        }

        private Sprite GenerateSpriteTile(int x, int y)
        {
            var s = new Sprite(this.Texture);
            s.Size = new Vector2(this.tileWidth, -this.tileHeight);
            s.SourceRectangle = new RectangleAdaptor(this.Tile(x, y));
            return s;
        }
    }
}
