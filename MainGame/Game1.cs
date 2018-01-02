using System;
using System.Threading.Tasks;
using Contracts;
using CraftingGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Prerelease.Main.Input;
using Renderer;
using Color = Contracts.Color;
using Vector2 = VectorMath.Vector2;

namespace Prerelease.Main
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public const bool Debug = true;

        private PerfCounter renderPerfCounter = new PerfCounter(15);
        private PerfCounter updatePerfCounter = new PerfCounter(15);

        private FrameCounter renderCounter = new FrameCounter();
        private FrameCounter updateCounter = new FrameCounter();

        private ISceene activeSceene = null;
        private readonly IMonoInput keyboard;
        private readonly MonoUIKeyboardInput uiKeyboard;
        private readonly IMonoInput[] controllers;
        private readonly InputMask uiInput = new InputMask("ui");
        private readonly InputMask inputMergeMask = new InputMask(null);
        private readonly InputMask[] inputMasks = new InputMask[4]
        {
            new InputMask("player1"),
            new InputMask("player2"),
            new InputMask("player3"),
            new InputMask("player4"),
        };
        private readonly ActionQueue actionQueue = new ActionQueue();
        private Action[] actionMap;
        
        private InputSet currentInputs = new InputSet();
        private Engine renderer;
        private UserInterface userInterface;
        private IFont debugFont;

        private const float fps = 60.0f;
        private const float msecPerFrame = 1000.0f / fps;

        public Game1()
        {
            IsMouseVisible = false;

            keyboard = new MonoKeyboardInput();
            uiKeyboard = new MonoUIKeyboardInput();
            controllers = new[]
            {
                new MonoControllerInput(PlayerIndex.One),
                new MonoControllerInput(PlayerIndex.Two),
                new MonoControllerInput(PlayerIndex.Three),
                new MonoControllerInput(PlayerIndex.Four),
            };

            var graphics = new GraphicsDeviceManager(this);
            graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";

            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)msecPerFrame);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SetupActionMap();
            base.Initialize();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            activeSceene?.Exiting();
        }

        private void SetupActionMap()
        {
            actionMap = new Action[Enum.GetNames(typeof(ActionType)).Length];

            actionMap[(int)ActionType.Quit] = this.Exit;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            renderer = new Engine(this.Content, this.GraphicsDevice);
            userInterface = new UserInterface(renderer);

            var scope = renderer.ActivateScope("Debug");
            debugFont = scope.LoadFont("ConsoleFont");

            this.activeSceene = new PlatformerSceene(new StreamProvider(), renderer, userInterface, actionQueue);
            this.activeSceene.Activate(uiInput, inputMasks);
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
            updatePerfCounter.Execute(() =>
            {
                updateCounter.Inc();
                var timestep = gameTime.ElapsedGameTime.Milliseconds / msecPerFrame;
                var time = (float)gameTime.TotalGameTime.TotalSeconds;
                var deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;

                uiInput.Apply(uiKeyboard.ReadInput());

                if (uiKeyboard.KeyPressed(Keys.Escape))
                {
                    activeSceene.Exiting();
                    Exit();
                }

                currentInputs = MergeInputs(keyboard, controllers[1]);
                inputMasks[0].Apply(currentInputs);
                inputMasks[1].Apply(controllers[0].ReadInput());
                inputMasks[2].Apply(controllers[2].ReadInput());
                inputMasks[3].Apply(controllers[3].ReadInput());
                activeSceene.Update(updateCounter, timestep);
                userInterface.Update(updateCounter, inputMasks);

                ProcessActions();

                base.Update(gameTime);
            });
        }

        /// <summary>
        /// Merges inputs from keyboard and controller to support both input types at the same time.
        /// </summary>
        /// <returns>The merged input set.</returns>
        private InputSet MergeInputs(params IMonoInput[] inputs)
        {
            inputMergeMask.Reset();
            foreach (var input in inputs)
            {
                inputMergeMask.Apply(input.ReadInput());
            }
            return inputMergeMask.Input;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            renderPerfCounter.Execute(() =>
            {
                renderCounter.Inc();

                activeSceene.Prerender(renderCounter, gameTime.TotalGameTime.TotalMilliseconds);

                activeSceene.Render(renderCounter, gameTime.TotalGameTime.TotalMilliseconds);
                userInterface.Render(renderCounter);

                if (Debug)
                {
                    renderer.Begin();
                    renderer.SetScissorRectangle(Vector2.Zero, renderer.GetDisplaySize());

                    var excess = updatePerfCounter.ExceedsThresshold || renderPerfCounter.ExceedsThresshold;
                    renderer.RenderText(
                        debugFont,
                        Vector2.Zero,
                        string.Format(
                            "U/R: {0:0.00}/{1:0.00}",
                            updatePerfCounter.AverageMsec,
                            renderPerfCounter.AverageMsec),
                        excess ? Color.Red : Color.Green);

                    var cursor = Vector2.Zero;
                    foreach (var diagnosticsString in activeSceene.DiagnosticsString())
                    {
                        cursor -= new Vector2(0, 20);

                        renderer.RenderText(
                            debugFont,
                            cursor,
                            diagnosticsString,
                            Color.Red);
                    }

                    renderer.End();
                }

                base.Draw(gameTime);
            });
        }

        private void ProcessActions()
        {
            while (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                actionMap[(int)action.Type]();
            }
        }
    }
}
