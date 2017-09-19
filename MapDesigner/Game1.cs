using System;
using System.IO;
using System.Reflection;
using Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Renderer;
using VectorMath;
using Color = Microsoft.Xna.Framework.Color;
using Point = VectorMath.Point;
using Vector2 = VectorMath.Vector2;

namespace MapDesigner
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const int TileWidth = 32;
        const int TileHeight = 32;
        const float PaletteScale = 2f;

        private readonly GraphicsDeviceManager graphics;
        private Engine renderer;
        private SpriteMap palette;
        private Map map;
        private ISprite mapSprite;
        private ISprite paletteSprite;

        private MouseState lastMouse;
        private float zoom = 2.0f;

        private readonly Vector2 paletteLayout = Vector2.Zero;
        private readonly Vector2 mapLayout = new Vector2(100, 30);
        private readonly Vector2 paletteTileSize = new Vector2(TileWidth, TileHeight);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            dragState = new DragState();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            lastMouse = Mouse.GetState();

            renderer = new Engine(this.Content, this.GraphicsDevice);

            // Load sprites from embedded texture.
            var assembly = typeof(Game1).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("MapDesigner.Content.sprites.hyptosis_tile-art-batch-1.png"))
            {
                palette = new SpriteMap(Texture2D.FromStream(graphics.GraphicsDevice, stream), TileWidth, TileHeight);
            }

            // Render the palette
            paletteSprite = renderer.RenderToTexture(TileWidth, 20 * TileHeight, () =>
            {
                var reference = new SpriteReference();
                reference.x = 0;
                renderer.Clear(Contracts.Color.Black);
                var offset = new Vector2(0, 0);
                for (int y = 0; y < 20; y++)
                {
                    reference.y = y;
                    var lastSprite = palette.Sprite(reference);
                    renderer.RenderOpagueSprite(lastSprite, offset, lastSprite.Size);
                    offset.Y += lastSprite.Size.Y;
                }
            });

            // Create map and render to texture.
            map = new Map(palette, TileWidth, TileHeight, 20, 20);
            mapSprite = map.Render(renderer);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private readonly DragState dragState;

        private SpriteReference stencil = new SpriteReference() { x = 0, y = 0 };

        protected override void Update(GameTime gameTime)
        {
            var mouseInput = Mouse.GetState();

            if (lastMouse.LeftButton == ButtonState.Pressed && mouseInput.LeftButton == ButtonState.Released)
            {
                // Was the palette clicked?
                if (mouseInput.Position.X >= 0 && mouseInput.Position.X < TileWidth*PaletteScale)
                {
                    // Find the palette index that was clicked.
                    var clickPosition = new Vector2(mouseInput.Position.X, mouseInput.Position.Y);
                    clickPosition -= paletteLayout;
                    clickPosition /= PaletteScale;
                    var paletteY = (int)Math.Floor(clickPosition.Y/TileHeight);

                    // Check that the new value is valid.
                    if (paletteY >= 0 && paletteY <= palette.SizeY)
                    {
                        stencil.y = paletteY;
                    }
                }
                else
                {
                    // User clicked the map, figure out where!
                    var clickPosition = GetRelativePosition(new Point(mouseInput.Position.X, mouseInput.Position.Y));
                    var mapIndex = map.GetIndexForPosition(clickPosition);

                    // Test if this was inside the map
                    if (map.PointInside(mapIndex))
                    {
                        // Update value of clicked map tile
                        map.Update(mapIndex, stencil);

                        // Re-render the map after it has been changed.
                        mapSprite = map.Render(renderer);
                    }
                }
            }

            dragState.Update(mouseInput);

            var newMouseWheelValue = mouseInput.ScrollWheelValue;
            zoom += (lastMouse.ScrollWheelValue - newMouseWheelValue)*0.01f;

            lastMouse = mouseInput;
            base.Update(gameTime);
        }

        private Vector2 GetRelativePosition(IReadonlyPoint position)
        {
            var relativePosition = new Vector2(position.X, position.Y);
            relativePosition -= mapLayout;
            relativePosition += dragState.Offset;
            relativePosition /= zoom;
            return relativePosition;
        }

        private Contracts.Color paletteHighlight = new Contracts.Color(0.5f, 0.5f, 0.5f, 0.5f);

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var clearColor = Color.CornflowerBlue;
            renderer.Begin();
            renderer.Clear(new Contracts.Color((float)clearColor.R / 255, (float)clearColor.G /255, (float)clearColor.B/ 255));

            var offset = mapLayout;
            offset -= (dragState.Offset);
            renderer.RenderOpagueSprite(mapSprite, offset, mapSprite.Size * zoom);

            // Render palette
            offset = new Vector2(paletteLayout);
            renderer.RenderOpagueSprite(paletteSprite, offset, paletteSprite.Size * PaletteScale);

            // Render something to show the currently selected palette entry.
            offset.Y += stencil.y*TileHeight*PaletteScale;
            renderer.RenderRectangle(offset, paletteTileSize * PaletteScale, paletteHighlight);

            renderer.End();

            base.Draw(gameTime);
        }
    }
}
