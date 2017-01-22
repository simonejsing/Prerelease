using Contracts;
using HubSceene;
using VectorMath;

namespace Prerelease.Main
{
    internal class UserInterface : IUserInterface
    {
        private const int MaxLines = 10;
        private const int TextHeight = 50;

        private readonly IRenderer Renderer;
        private readonly IFont Font;
        private IDialog currentDialog = new CompletedDialog();

        public UserInterface(IRenderer renderer)
        {
            Renderer = renderer;
            var scope = renderer.ActivateScope("UI");
            Font = scope.LoadFont("ConsoleFont");
        }

        public void BeginDialog(IDialog dialog)
        {
            // TODO: What if there is an active dialog already?

            currentDialog = dialog;

            // TODO: Page the current dialog in case it has many lines
        }

        public void Render()
        {
            if (currentDialog.Completed)
            {
                return;
            }
            
            // Align center vertically
            var viewport = Renderer.GetViewport();
            var textBox = new Vector2(200, MaxLines * TextHeight);
            var position = (viewport - textBox) / 2f;

            // TODO: Clear the background area by drawing a large sprite

            // Render lines
            foreach (var line in currentDialog.Lines)
            {
                Renderer.RenderText(Font, position, line, Color.Green, 0f, Vector2.Zero, new Vector2(3f, 3f));
                position -= new Vector2(0, TextHeight);
            }
        }
    }
}