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
        private SceeneObject player, door;
        private Vector2 playerPosition = new Vector2();
        private Vector2 doorPosition = new Vector2();

        public HubSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue) : base("Hub", renderer, ui, actionQueue)
        {
            player = new SceeneObject(actionQueue);
            door = new SceeneObject(actionQueue);
            //door.Action = new GameAction(ActionType.Teleport);
        }

        public override void ProcessInput(float timestep, InputMask inputMask)
        {
            if (UI.HasActiveDialog)
            {
                return;
            }

            if (inputMask.Input.Right)
            {
                playerPosition.X += 1;
            }
            if (inputMask.Input.Down)
            {
                playerPosition.Y -= 1;
            }
            if (inputMask.Input.Select)
            {
                InvokeObjectUnder(player);
            }
            inputMask.Reset();
        }

        public override void Activate()
        {
            base.Activate();

            // Load content in active scope.
            player.Sprite = LoadSprite("Chicken");
            playerPosition = new Vector2(0, 0);
            player.Sprite.Size = new Vector2(30, -30);
            door.Sprite = LoadSprite("Door");
            doorPosition = new Vector2(400, -200);
            door.Sprite.Size = new Vector2(30, -60);

            // Start dialog
            var dialog = new Dialog(ActionQueue, Dialog.ReadEmbeddedDialog(typeof(HubSceene), "HubSceene.Content.Dialog.Dialog1.txt"));

            UI.BeginDialog(dialog);
        }

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);
            Renderer.RenderOpagueSprite(door.Sprite, doorPosition, door.Sprite.Size);
            Renderer.RenderOpagueSprite(player.Sprite, playerPosition, player.Sprite.Size);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void InvokeObjectUnder(SceeneObject obj)
        {
/*            if (obj.Sprite.Rect.PointInside(door.Sprite.Rect.Center))
            {
                door.Activate(obj);
            }*/
        }
    }
}
