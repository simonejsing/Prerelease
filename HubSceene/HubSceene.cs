using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Contracts;
using VectorMath;

namespace HubSceene
{
    public class HubSceene : Sceene
    {
        private ISprite playerSprite;
        private Vector2 playerPosition = new Vector2(0, 0);

        public HubSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue) : base("Hub", renderer, ui, actionQueue)
        {
        }

        public override void ProcessInput(double gameTimeMsec, InputMask inputMask)
        {
            if (inputMask.Input.Right)
            {
                playerPosition += new Vector2(1, 0);
            }
            if (inputMask.Input.Down)
            {
                playerPosition += new Vector2(0, -1);
            }
            inputMask.Reset();
        }

        public override void Activate()
        {
            base.Activate();

            // Load content in active scope.
            playerSprite = LoadSprite("player");

            // Start dialog
            var dialog = new Dialog(ActionQueue, Dialog.ReadEmbeddedDialog(typeof(HubSceene), "HubSceene.Content.Dialog.Dialog1.txt"));

            UI.BeginDialog(dialog);
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);
            Renderer.RenderOpagueSprite(playerSprite, playerPosition);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}
