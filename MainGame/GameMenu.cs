﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Renderer;
using VectorMath;

namespace Prerelease.Main
{
    class GameMenu : Sceene
    {
        private const float TextHeight = 50f;

        private IFont menuFont;
        private int selectedOption = 0;
        private Vector2 menuPosition, viewport, textBox;

        private InputMask MenuInputMask;

        private readonly string[] mainMenu = new[]
        {
            "New game",
            "Options",
            "Quit"
        };

        public GameMenu(Engine renderer, ActionQueue actionQueue) : base("GameMenu", renderer, null, actionQueue)
        {
        }

        private int resetFrameCounter = 0;
        public override void Update(FrameCounter counter, float timestep)
        {
            if (resetFrameCounter > 0)
            {
                resetFrameCounter--;
                MenuInputMask.Reset();
                return;
            }

            if (MenuInputMask.Input.Up)
            {
                selectedOption = selectedOption == 0 ? 0 : selectedOption - 1;
                resetFrameCounter = 10;
            }
            if (MenuInputMask.Input.Down)
            {
                selectedOption = selectedOption == mainMenu.Length - 1 ? mainMenu.Length - 1 : selectedOption + 1;
                resetFrameCounter = 10;
            }
            if (MenuInputMask.Input.Select)
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

        public override void Activate(InputMask uiInput, InputMask[] inputMasks)
        {
            base.Activate(uiInput, inputMasks);
            MenuInputMask = inputMasks[0];
            menuFont = LoadFont("ConsoleFont");

            // Align center vertically
            viewport = new Vector2(Renderer.GetDisplaySize());
            textBox = new Vector2(200, -mainMenu.Length * TextHeight);
        }

        public override void Render(FrameCounter counter, double gameTimeMsec)
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
