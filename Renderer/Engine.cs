using System;
using System.Collections.Generic;
using Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VectorMath;
using IReadonlyVector = VectorMath.IReadonlyVector;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Renderer
{
    public class Engine : IRenderer
    {
        private VectorMath.Vector2 viewport;
        private GraphicsDevice device;
        private SpriteLibrary sprites;
        private FontLibrary fonts;
        private SpriteBatch spriteBatch;
        private Transformation transform;
        private Texture2D Pixel;

        public Engine(ContentManager manager, GraphicsDevice device)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.device = device;
            this.spriteBatch = new SpriteBatch(device);

            // Create 1x1 white pixel
            Pixel = new Texture2D(this.device, 1, 1);
            Pixel.SetData(new[] {Color.White});

            sprites = new SpriteLibrary(manager);
            fonts = new FontLibrary(manager);

            this.viewport = new VectorMath.Vector2(device.DisplayMode.Width, device.DisplayMode.Height);
            ResetTransform();
        }

        public IReadonlyVector GetViewport()
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
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        }

        public void End()
        {
            spriteBatch.End();
        }

        public void ResetTransform()
        {
            transform = new Transformation();
        }

        public void Scale(float x, float y)
        {
            transform.Scale(x, y);
        }

        public void RenderVector(IReadonlyVector origin, IReadonlyVector vector, Contracts.Color color, float thickness = 1.0f)
        {
            var to = new VectorMath.Vector2(origin.X + vector.X, origin.Y + vector.Y);
            RenderLine(
                transform.Apply(origin),
                transform.Apply(to),
                color,
                thickness);
        }

        public void RenderLine(IReadonlyVector point1, IReadonlyVector point2, Contracts.Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = VectorMath.Vector2.DistanceBetween(new VectorMath.Vector2(point1), new VectorMath.Vector2(point2));

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(point1, distance, angle, color, thickness);
        }

        public void DrawLine(IReadonlyVector point, float length, float angle, Contracts.Color color, float thickness)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(Pixel,
                             ToXnaVector(point),
                             null,
                             ToXnaColor(color),
                             angle,
                             ToXnaVector(VectorMath.Vector2.Zero),
                             ToXnaVector(new VectorMath.Vector2(length, thickness)),
                             SpriteEffects.None,
                             0);
        }

        public void RenderPixel(IReadonlyVector position, Contracts.Color color)
        {
            spriteBatch.Draw(Pixel, ToXnaVector(transform.Apply(position)), ToXnaColor(color));
        }

        public void RenderRectangle(IReadonlyVector position, IReadonlyVector size, Contracts.Color color)
        {
            Rectangle rect = new Rectangle(ToXnaPoint(transform.Apply(position)), ToXnaPoint(transform.Apply(size)));
            spriteBatch.Draw(Pixel, rect, ToXnaColor(color));
        }

        public void RenderOpagueSprite(ISprite sprite, IReadonlyVector position, IReadonlyVector size, bool flipHorizontally = false)
        {
            var s = (Sprite) sprite;
            var effect = flipHorizontally ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle rect = new Rectangle(ToXnaPoint(transform.Apply(position)), ToXnaPoint(transform.Apply(size)));
            spriteBatch.Draw(s.Texture, rect, ToXnaRect(s.SourceRectangle), Color.White, 0f, Vector2.Zero, effect, 0f);
        }

        private static Rectangle? ToXnaRect(IReadonlyRectangle rectangle)
        {
            if (rectangle == null)
                return null;

            var adaptor = rectangle as RectangleAdaptor;
            if (adaptor != null)
            {
                return adaptor.Rect;
            }

            throw new NotImplementedException();
        }

        public void RenderText(IFont font, IReadonlyVector position, string text, Contracts.Color color)
        {
            RenderText(font, position, text, color, 0f, VectorMath.Vector2.Zero, new VectorMath.Vector2(1f, 1f));
        }

        public void RenderText(IFont font, IReadonlyVector position, string text, Contracts.Color color, float rotation,
            IReadonlyVector origin, IReadonlyVector scale, float layerDepth = 0f)
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
            return new Color(color.r, color.g, color.b, color.a);
        }

        private Microsoft.Xna.Framework.Vector2 ToXnaVector(IReadonlyVector position)
        {
            return new Microsoft.Xna.Framework.Vector2(position.X, position.Y);
        }

        private Microsoft.Xna.Framework.Point ToXnaPoint(IReadonlyVector position)
        {
            return new Microsoft.Xna.Framework.Point((int)position.X, (int)position.Y);
        }

        public ISprite RenderToTexture(int width, int height, Action renderAction)
        {
            var renderTarget = new RenderTarget2D(device, width, height, false, device.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            device.SetRenderTarget(renderTarget);
            Begin();
            renderAction();
            End();
            device.SetRenderTarget(null);
            return new Sprite(renderTarget);
        }
    }
}
