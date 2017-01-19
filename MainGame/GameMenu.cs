using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Prerelease.Main.Render;
using VectorMath;

namespace Prerelease.Main
{
    class GameMenu : Sceene
    {
        private const float textHeight = 50f;

        private IFont menuFont;
        private int selectedOption = 0;

        private string[] mainMenu = new[]
        {
            "New game",
            "Options",
            "Quit"
        };

        public GameMenu(Renderer renderer, ActionQueue actionQueue) : base("GameMenu", renderer, actionQueue)
        {
        }

        private int resetFrameCounter = 0;
        public override void ProcessInput(double gameTimeMsec, InputMask inputMask)
        {
            if (resetFrameCounter > 0)
            {
                resetFrameCounter--;
                inputMask.Reset();
                return;
            }

            if (inputMask.Input.Up)
            {
                selectedOption = selectedOption == 0 ? 0 : selectedOption - 1;
                resetFrameCounter = 15;
            }
            if (inputMask.Input.Down)
            {
                selectedOption = selectedOption == mainMenu.Length - 1 ? mainMenu.Length - 1 : selectedOption + 1;
                resetFrameCounter = 15;
            }
            if (inputMask.Input.Select)
            {
                DispatchCurrentMenuAction();
                resetFrameCounter = 15;
            }
        }

        private void DispatchCurrentMenuAction()
        {
            switch (selectedOption)
            {
                case 0:
                    // Dispatch new game action
                    ActionQueue.Enqueue(new GameAction(ActionType.NewGame));
                    return;
                case 1:
                    return;
                case 2:
                    // Dispatch game quit action to main
                    ActionQueue.Enqueue(new GameAction(ActionType.Quit));
                    return;
            }
        }

        public override void Activate()
        {
            base.Activate();
            menuFont = LoadFont("ConsoleFont");
        }

        public override void Render(double gameTimeMsec)
        {
            // Align center vertically
            var viewport = Renderer.GetViewport();
            var textBox = new Vector2(200, -mainMenu.Length * textHeight);
            var position = (viewport - textBox) / 2f;

            for(var i = 0; i < mainMenu.Length; i++)
            {
                Renderer.RenderText(menuFont, position, mainMenu[i], Color.White, 0f, Vector2.Zero, new Vector2(2f, 2f));
                if (i == selectedOption)
                {
                    Renderer.RenderText(menuFont, position - new Vector2(30, 0), "*", Color.Red, 0f, Vector2.Zero, new Vector2(3f, 3f));
                }
                position -= new Vector2(0, textHeight);
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}
