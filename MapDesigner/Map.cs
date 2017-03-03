using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Renderer;
using VectorMath;

namespace MapDesigner
{
    class Map
    {
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        private SpriteMap palette;
        private SpriteReference[,] map;

        public Map(SpriteMap palette, int tileWidth, int tileHeight, int sizeX, int sizeY)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            SizeX = sizeX;
            SizeY = sizeY;
            this.palette = palette;
            map = new SpriteReference[SizeX, SizeY];
        }

        public ISprite Render(Engine renderer)
        {
            return renderer.RenderToTexture(SizeX * TileWidth, SizeY * TileHeight, () =>
            {
                renderer.Clear(Color.Black);
                var offset = new Vector2(0, 0);
                for (int x = 0; x < SizeX; x++)
                {
                    ISprite lastSprite = null;
                    for (int y = 0; y < SizeY; y++)
                    {
                        lastSprite = palette.Sprite(map[x, y]);
                        renderer.RenderOpagueSprite(lastSprite, offset);
                        offset.Y += lastSprite.Size.Y;
                    }

                    if (lastSprite == null)
                    {
                        throw new InvalidOperationException("lastSprite is null");
                    }
                    offset.X += lastSprite.Size.X;
                    offset.Y = 0;
                }
            });
        }


        public void Update(Point index, SpriteReference reference)
        {
            if (!PointInside(index))
            {
                throw new InvalidOperationException(string.Format("The specified update index {0} is outside the range of the map.", index));
            }
            map[index.X, index.Y] = reference;
        }

        public bool PointInside(Point index)
        {
            return index.X >= 0 && index.X < SizeX && index.Y >= 0 && index.Y < SizeY;
        }

        public Point GetIndexForPosition(Vector2 position)
        {
            return new Point((int)Math.Floor(position.X / TileWidth), (int)Math.Floor(position.Y / TileHeight));
        }
    }
}
