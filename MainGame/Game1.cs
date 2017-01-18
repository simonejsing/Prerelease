using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = VectorMath.Vector2;

namespace Prerelease.Main
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public const bool DEBUG = true;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private GameMenu menu;
        private ISceene activeSceene = null;
        private readonly IMonoInput playerInput;
        private readonly InputMask inputMask = new InputMask();

        private Renderer renderer;
        private int updateFrame = 0;
        private int renderFrame = 0;

        public Game1()
        {
            //playerInput = new MonoKeyboardInput();
            playerInput = new MonoControllerInput(PlayerIndex.One);
            graphics = new GraphicsDeviceManager(this);
            graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //player = new Player() {Position = new Vector2(20, -150)};

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

            renderer = new Renderer(this.Content, this.GraphicsDevice);

            this.menu = new GameMenu(renderer, spriteBatch);
            this.activeSceene = this.menu;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            updateFrame++;

            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            var deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;

            playerInput.Update();
            inputMask.Apply(playerInput);
            activeSceene.ProcessInput(gameTime, inputMask);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            renderFrame++;

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            activeSceene.Render(gameTime);

            if (DEBUG)
            {
                renderer.RenderText(
                    spriteBatch,
                    Vector2.Zero,
                    string.Format(
                        "U/D: {0}/{1} [{2}{3}{4}]",
                        updateFrame,
                        renderFrame,
                        inputMask.Input.Up ? "U" : (playerInput.Up() ? "u" : "-"),
                        inputMask.Input.Down ? "D" : (playerInput.Down() ? "d" : "-"),
                        inputMask.Input.Select ? "S" : (playerInput.Select() ? "s" : "-")),
                    Color.Red);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
