using System;
using Contracts;
using HubSceene;
using VectorMath;

namespace Prerelease.Main
{
    public class PlatformerSceene : Sceene
    {
        private SceeneObject player;
        private Vector2 playerPosition;

        public PlatformerSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue)
            : base("Platformer", renderer, ui, actionQueue)
        {
            player = new SceeneObject(actionQueue);
        }

        public override void Activate()
        {
            base.Activate();

            // Load content in active scope.
            player.Sprite = LoadSprite("Chicken");
            playerPosition = new Vector2(0, 0);
            player.Sprite.Size = new Vector2(30, 30);
        }

        public override void ProcessInput(float timestep, InputMask inputMask)
        {
            if (inputMask.Input.Right)
            {
                playerPosition.X += 3 * timestep;
            }
            if (inputMask.Input.Down)
            {
                playerPosition.Y -= 3 * timestep;
            }
            if (inputMask.Input.Select)
            {
            }
            inputMask.Reset();
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);
            Renderer.RenderOpagueSprite(player.Sprite, playerPosition, player.Sprite.Size);
        }
    }
}