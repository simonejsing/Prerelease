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
        private const float TextHeight = 50f;

        private IFont menuFont;
        private int selectedOption = 0;
        private Vector2 menuPosition, viewport, textBox;

        private readonly string[] mainMenu = new[]
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
                resetFrameCounter = 10;
            }
            if (inputMask.Input.Down)
            {
                selectedOption = selectedOption == mainMenu.Length - 1 ? mainMenu.Length - 1 : selectedOption + 1;
                resetFrameCounter = 10;
            }
            if (inputMask.Input.Select)
            {
                DispatchCurrentMenuAction();
                resetFrameCounter = 10;
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

            // Align center vertically
            viewport = Renderer.GetViewport();
            textBox = new Vector2(200, -mainMenu.Length * TextHeight);
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);

            menuPosition = (viewport - textBox) / 2f;

            for (var i = 0; i < mainMenu.Length; i++)
            {
                Renderer.RenderText(menuFont, menuPosition, mainMenu[i], Color.White, 0f, Vector2.Zero, new Vector2(2f, 2f));
                if (i == selectedOption)
                {
                    Renderer.RenderText(menuFont, menuPosition - new Vector2(30, 0), "*", Color.Red, 0f, Vector2.Zero, new Vector2(3f, 3f));
                }
                menuPosition -= new Vector2(0, TextHeight);
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}
