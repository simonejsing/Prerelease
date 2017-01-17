﻿using System;
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
        private const float textHeight = 30f;
        private Renderer renderer;
        private SpriteBatch spriteBatch;

        private int selectedOption = 0;

        private string[] mainMenu = new[]
        {
            "New game",
            "Options",
            "Quit"
        };

        public GameMenu(Renderer renderer, SpriteBatch spriteBatch)
        {
            this.renderer = renderer;
            this.spriteBatch = spriteBatch;
        }


        public void ProcessInput(IMonoInput inputs)
        {
            // TODO: Throttle inputs.
            if (inputs.Up())
            {
                selectedOption = selectedOption == 0 ? mainMenu.Length - 1 : selectedOption - 1;
            }
            if (inputs.Down())
            {
                selectedOption = selectedOption == mainMenu.Length - 1 ? 0 : selectedOption + 1;
            }
            if (inputs.Select())
            {
                DispatchCurrentMenuAction();
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
                    return;
            }
        }

        public void Activate()
        {
        }

        public void Render()
        {
            // Align center vertically
            var viewport = renderer.GetViewport();
            var textBox = new Vector2(200, -mainMenu.Length * textHeight);
            var position = (viewport - textBox) / 2f;

            /*renderer.RenderOpagueSprite(spriteBatch, SpriteLibrary.SpriteIdentifier.Player, new Vector2(1, -1));
            renderer.RenderOpagueSprite(spriteBatch, SpriteLibrary.SpriteIdentifier.Player, new Vector2(2736-40, -1));
            renderer.RenderOpagueSprite(spriteBatch, SpriteLibrary.SpriteIdentifier.Player, new Vector2(1, -1824+240));
            renderer.RenderOpagueSprite(spriteBatch, SpriteLibrary.SpriteIdentifier.Player, new Vector2(2736-40, -1824+40));*/
            for(var i = 0; i < mainMenu.Length; i++)
            {
                renderer.RenderText(spriteBatch, position, mainMenu[i], Color.White);
                if (i == selectedOption)
                {
                    renderer.RenderText(spriteBatch, position - new Vector2(30, 0), "*", Color.White);
                }
                position -= new Vector2(0, textHeight);
            }
        }

        public void Deactivate()
        {
        }
    }
}
