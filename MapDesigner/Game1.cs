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
        private Renderer.Engine renderer;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteMap map;
        private ISprite tiledMap;

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
            renderer = new Renderer.Engine(this.Content, this.GraphicsDevice);

            // Load sprites from textures.
            var assembly = typeof(Game1).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("MapDesigner.Content.sprites.hyptosis_tile-art-batch-1.png"))
            {
                map = new SpriteMap(Texture2D.FromStream(graphics.GraphicsDevice, stream), 32, 32);
            }

            tiledMap = renderer.RenderToTexture(500, 500, () =>
            {
                renderer.Clear(Contracts.Color.Black);
                var offset = new VectorMath.Vector2(30, 30);
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        var s = map.Sprite(x, y);
                        renderer.RenderOpagueSprite(s, offset);
                        offset.Y += s.Size.Y;
                        offset.Y += 15;
                    }

                    offset.X += map.Sprite(x, 9).Size.X;
                    offset.X += 15;
                    offset.Y = 30;
                }
            });

            tiledMap.Size *= 2.5f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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

        protected override void Update(GameTime gameTime)
        {
            var mouseInput = Mouse.GetState();

            dragState.Update(mouseInput);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var clearColor = Color.CornflowerBlue;
            renderer.Begin();
            renderer.Clear(new Contracts.Color((float)clearColor.R / 255, (float)clearColor.G /255, (float)clearColor.B/ 255));

            var offset = new VectorMath.Vector2(30, 30);
            offset -= (dragState.Offset);
            renderer.RenderOpagueSprite(tiledMap, offset);

            renderer.End();

            base.Draw(gameTime);
        }
    }
}
