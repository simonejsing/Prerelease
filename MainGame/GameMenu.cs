using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = VectorMath.Vector2;

namespace Prerelease.Main
{
    class GameMenu : ISceene
    {
        private const float textHeight = 50f;
        private Renderer renderer;
        private SpriteBatch spriteBatch;
        private ActionQueue actionQueue;

        private int selectedOption = 0;

        private string[] mainMenu = new[]
        {
            "New game",
            "Options",
            "Quit"
        };

        public GameMenu(Renderer renderer, SpriteBatch spriteBatch, ActionQueue actionQueue)
        {
            this.renderer = renderer;
            this.spriteBatch = spriteBatch;
            this.actionQueue = actionQueue;
        }

        private int resetFrameCounter = 0;
        public void ProcessInput(GameTime gameTime, InputMask inputMask)
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
                    return;
                case 1:
                    return;
                case 2:
                    // Dispatch game quit action to main
                    actionQueue.Enqueue(new GameAction(ActionType.Quit));
                    return;
            }
        }

        public void Activate()
        {
        }

        public void Render(GameTime gameTime)
        {
            // Align center vertically
            var viewport = renderer.GetViewport();
            var textBox = new Vector2(200, -mainMenu.Length * textHeight);
            var position = (viewport - textBox) / 2f;

            for(var i = 0; i < mainMenu.Length; i++)
            {
                renderer.RenderText(spriteBatch, position, mainMenu[i], Color.White, 0f, Vector2.Zero, new Vector2(2f, 2f));
                if (i == selectedOption)
                {
                    renderer.RenderText(spriteBatch, position - new Vector2(30, 0), "*", Color.Red, 0f, Vector2.Zero, new Vector2(3f, 3f));
                }
                position -= new Vector2(0, textHeight);
            }
        }

        public void Deactivate()
        {
        }
    }
}
