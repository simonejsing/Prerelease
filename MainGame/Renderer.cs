using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VectorMath;
using Vector2 = VectorMath.Vector2;

namespace Prerelease.Main
{
    class Renderer
    {
        private Vector2 viewport;
        private SpriteFont DefaultFont;
        private SpriteLibrary sprites;
        private Transformation transform;
        private Texture2D Pixel;

        public Renderer(ContentManager manager, GraphicsDevice device)
        {
            Pixel = manager.Load<Texture2D>(@"Sprites\pixel");

            sprites = new SpriteLibrary(manager);
            sprites.LoadSprite(SpriteLibrary.SpriteIdentifier.Player, @"Sprites\player");
            sprites.LoadSprite(SpriteLibrary.SpriteIdentifier.Block, @"Sprites\block");
            sprites.LoadSprite(SpriteLibrary.SpriteIdentifier.Spikes, @"Sprites\spikes");

            DefaultFont = manager.Load<SpriteFont>("DefaultFont");

            this.viewport = new Vector2(device.Viewport.Width, -device.Viewport.Height);
            ResetTransform(device);
        }

        public Vector2 GetViewport()
        {
            return this.viewport;
        }

        private void ResetTransform(GraphicsDevice device)
        {
            // Mirror y-axis
            transform = new Transformation();
            transform.Scale(1, -1);

            // Scale viewport to 100x100
            //transform.Scale(device.Viewport.Width / 100f, device.Viewport.Height / 100f);
        }

        public void DrawVector(SpriteBatch spriteBatch, Vector2 origin, Vector2 vector, Color color, float thickness = 1.0f)
        {
            var to = origin + vector;
            RenderLine(
                spriteBatch,
                transform.Apply(origin),
                transform.Apply(to),
                color,
                thickness);
        }

        public void RenderLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.DistanceBetween(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(Pixel,
                             ToXnaVector(point),
                             null,
                             color,
                             angle,
                             ToXnaVector(Vector2.Zero),
                             ToXnaVector(new Vector2(length, thickness)),
                             SpriteEffects.None,
                             0);
        }

        public void RenderPixel(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(Pixel, ToXnaVector(transform.Apply(position)), color);
        }

        public void RenderOpagueSprite(SpriteBatch spriteBatch, SpriteLibrary.SpriteIdentifier spriteIdentifier, Vector2 position)
        {
            var xnaVector = ToXnaVector(transform.Apply(position));
            spriteBatch.Draw(sprites.GetSprite(spriteIdentifier), xnaVector, Color.White);
        }

        public void RenderText(SpriteBatch spriteBatch, Vector2 origin, string text, Color color)
        {
            spriteBatch.DrawString(
                DefaultFont,
                text,
                ToXnaVector(transform.Apply(origin)),
                color);
        }

        private Microsoft.Xna.Framework.Vector2 ToXnaVector(Vector2 position)
        {
            return new Microsoft.Xna.Framework.Vector2(position.X, position.Y);
        }
    }
}
