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
        private readonly ISprite[,] spriteCache;

        public Texture2D Texture { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }
        public int SizeX { get; }
        public int SizeY { get; }

        public SpriteMap(Texture2D texture, int spriteTileWidth, int spriteTileHeight)
        {
            this.Texture = texture;
            this.TileWidth = spriteTileWidth;
            this.TileHeight = spriteTileHeight;

            this.SizeX = this.Texture.Width / this.TileWidth;
            this.SizeY = this.Texture.Height / this.TileHeight;
            this.spriteCache = new ISprite[SizeX,SizeY];
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    this.spriteCache[x, y] = null;
                }
            }
        }

        public Rectangle Tile(int x, int y)
        {
            return new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
        }

        public ISprite Sprite(SpriteReference reference)
        {
            if (reference.x >= this.SizeX || reference.y >= this.SizeY)
            {
                throw new Exception("Invalid sprite index into sprite map.");
            }

            if (this.spriteCache[reference.x, reference.y] == null)
            {
                this.spriteCache[reference.x, reference.y] = GenerateSpriteTile(reference.x, reference.y);
            }

            return this.spriteCache[reference.x, reference.y];
        }

        private Sprite GenerateSpriteTile(int x, int y)
        {
            var s = new Sprite(this.Texture);
            s.Size = new Vector2(this.TileWidth, this.TileHeight);
            s.SourceRectangle = new RectangleAdaptor(this.Tile(x, y));
            return s;
        }
    }
}
