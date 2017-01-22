using System;
using Contracts;
using HubSceene;
using Microsoft.Xna.Framework;
using Prerelease.Main.Input;
using Prerelease.Main.Render;
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

        private ISceene activeSceene = null;
        private readonly IMonoInput keyboard;
        private readonly IMonoInput controller;
        private readonly InputMask inputMergeMask = new InputMask();
        private readonly InputMask inputMask = new InputMask();
        private readonly ActionQueue actionQueue = new ActionQueue();
        private Action[] actionMap;
        
        private InputSet currentInputs = new InputSet();
        private Renderer renderer;
        private UserInterface userInterface;
        private IFont debugFont;
        private int updateFrame = 0;
        private int renderFrame = 0;

        public Game1()
        {
            keyboard = new MonoKeyboardInput();
            controller = new MonoControllerInput(PlayerIndex.One);

            var graphics = new GraphicsDeviceManager(this);
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
            SetupActionMap();
            base.Initialize();
        }

        private void SetupActionMap()
        {
            actionMap = new Action[Enum.GetNames(typeof(ActionType)).Length];

            actionMap[(int)ActionType.Quit] = this.Exit;
            actionMap[(int)ActionType.NewGame] = this.NewGame;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            renderer = new Renderer(this.Content, this.GraphicsDevice);
            userInterface = new UserInterface(renderer);

            var scope = renderer.ActivateScope("Debug");
            debugFont = scope.LoadFont("ConsoleFont");

            this.activeSceene = LoadSceene("Menu");
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

            currentInputs = MergeInputs();
            inputMask.Apply(currentInputs);
            activeSceene.ProcessInput(gameTime.TotalGameTime.TotalMilliseconds, inputMask);

            ProcessActions();

            base.Update(gameTime);
        }

        /// <summary>
        /// Merges inputs from keyboard and controller to support both input types at the same time.
        /// </summary>
        /// <returns>The merged input set.</returns>
        private InputSet MergeInputs()
        {
            inputMergeMask.Reset();
            inputMergeMask.Apply(keyboard.ReadInput());
            inputMergeMask.Apply(controller.ReadInput());
            return inputMergeMask.Input;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            renderFrame++;

            renderer.Begin();

            activeSceene.Render(gameTime.TotalGameTime.TotalMilliseconds);
            userInterface.Render();

            if (Debug)
            {
                renderer.RenderText(
                    debugFont,
                    Vector2.Zero,
                    string.Format(
                        "U/D: {0}/{1} [{2}{3}{4}{5}{6}]",
                        updateFrame,
                        renderFrame,
                        inputMask.Input.Left ? "L" : (currentInputs.Left ? "l" : "-"),
                        inputMask.Input.Right ? "R" : (currentInputs.Right ? "r" : "-"),
                        inputMask.Input.Up ? "U" : (currentInputs.Up ? "u" : "-"),
                        inputMask.Input.Down ? "D" : (currentInputs.Down ? "d" : "-"),
                        inputMask.Input.Select ? "S" : (currentInputs.Select ? "s" : "-")),
                    Color.Red);
            }

            renderer.End();

            base.Draw(gameTime);
        }

        private void NewGame()
        {
            this.activeSceene = LoadSceene("Hub");
        }

        private ISceene LoadSceene(string sceeneName)
        {
            activeSceene?.Deactivate();
            var sceene = CreateSceene(sceeneName);
            sceene.Activate();
            return sceene;
        }

        private ISceene CreateSceene(string sceeneName)
        {
            switch (sceeneName)
            {
                case "Menu":
                    return new GameMenu(renderer, actionQueue);
                case "Hub":
                    return new HubSceeneFactory().Create(renderer, userInterface, actionQueue);
                default:
                    throw new Exception(string.Format("Attempt to load unknown sceene '{0}'.", sceeneName));
            }
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
