using Contracts;
using Microsoft.Xna.Framework.Graphics;
using VectorMath;

namespace Renderer
{
    public class Sprite : ISprite
    {
        public Texture2D Texture { get; private set; }
        public IReadonlyRectangle SourceRectangle { get; set; }
        public Vector2 Size { get; set; }

        public Sprite(Texture2D texture)
        {
            this.Texture = texture;
            this.Size = new Vector2(texture.Width, texture.Height);
        }
    }
}
