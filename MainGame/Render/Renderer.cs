using System;
using System.Collections.Generic;
using Contracts;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = VectorMath.Vector2;
using Color = Microsoft.Xna.Framework.Color;

namespace Prerelease.Main.Render
{
    class Renderer : IRenderer
    {
        private Vector2 viewport;
        private GraphicsDevice device;
        private SpriteLibrary sprites;
        private FontLibrary fonts;
        private SpriteBatch spriteBatch;
        private Transformation transform;
        private Texture2D Pixel;

        public Renderer(ContentManager manager, GraphicsDevice device)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.device = device;
            this.spriteBatch = new SpriteBatch(device);
            Pixel = manager.Load<Texture2D>(@"Sprites\pixel");

            sprites = new SpriteLibrary(manager);
            sprites.Load("player");
            sprites.Load("block");
            sprites.Load("spikes");

            fonts = new FontLibrary(manager);

            this.viewport = new Vector2(device.Viewport.Width, -device.Viewport.Height);
            ResetTransform();
        }

        public Vector2 GetViewport()
        {
            return this.viewport;
        }

        public void Clear(Contracts.Color color)
        {
            device.Clear(ToXnaColor(color));
        }

        public void Begin()
        {
            spriteBatch.Begin();
        }

        public void End()
        {
            spriteBatch.End();
        }

        private void ResetTransform()
        {
            // Mirror y-axis
            transform = new Transformation();
            transform.Scale(1, -1);

            // Scale viewport to 100x100
            //transform.Scale(device.Viewport.Width / 100f, device.Viewport.Height / 100f);
        }

        public void DrawVector(Vector2 origin, Vector2 vector, Contracts.Color color, float thickness = 1.0f)
        {
            var to = origin + vector;
            RenderLine(
                transform.Apply(origin),
                transform.Apply(to),
                color,
                thickness);
        }

        public void RenderLine(Vector2 point1, Vector2 point2, Contracts.Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.DistanceBetween(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(point1, distance, angle, color, thickness);
        }

        public void DrawLine(Vector2 point, float length, float angle, Contracts.Color color, float thickness)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(Pixel,
                             ToXnaVector(point),
                             null,
                             ToXnaColor(color),
                             angle,
                             ToXnaVector(Vector2.Zero),
                             ToXnaVector(new Vector2(length, thickness)),
                             SpriteEffects.None,
                             0);
        }

        public void RenderPixel(Vector2 position, Contracts.Color color)
        {
            spriteBatch.Draw(Pixel, ToXnaVector(transform.Apply(position)), ToXnaColor(color));
        }

        public void RenderOpagueSprite(ISprite sprite, Vector2 position)
        {
            var xnaVector = ToXnaVector(transform.Apply(position));
            spriteBatch.Draw(((Sprite)sprite).Texture, xnaVector, Color.White);
        }

        public void RenderText(IFont font, Vector2 position, string text, Contracts.Color color)
        {
            RenderText(font, position, text, color, 0f, Vector2.Zero, new Vector2(1f, 1f));
        }

        public void RenderText(IFont font, Vector2 position, string text, Contracts.Color color, float rotation,
            Vector2 origin, Vector2 scale, float layerDepth = 0f)
        {
            spriteBatch.DrawString(
                ((Font) font).SpriteFont,
                text,
                ToXnaVector(transform.Apply(position)),
                ToXnaColor(color),
                rotation,
                ToXnaVector(transform.Apply(origin)),
                ToXnaVector(scale),
                SpriteEffects.None,
                layerDepth);
        }

        private readonly Dictionary<string, IRenderScope> scopes = new Dictionary<string, IRenderScope>();

        public IRenderScope ActivateScope(string scopeName)
        {
            // Create scope if it does not exist
            if (!scopes.ContainsKey(scopeName))
            {
                scopes.Add(scopeName, CreateScope());
            }

            return scopes[scopeName];
        }

        public void DeactivateScope(string scopeName)
        {
        }

        private IRenderScope CreateScope()
        {
            return new RenderScope(this.fonts, this.sprites);
        }

        private Color ToXnaColor(Contracts.Color color)
        {
            return new Color(color.r, color.g, color.b);
        }

        private Microsoft.Xna.Framework.Vector2 ToXnaVector(Vector2 position)
        {
            return new Microsoft.Xna.Framework.Vector2(position.X, position.Y);
        }
    }
}
