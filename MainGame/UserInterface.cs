using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Contracts;
using VectorMath;

namespace Prerelease.Main
{
    internal class UserInterface : IUserInterface
    {
        private const int CharacterWidth = 27;
        private const int TextHeight = 50;
        private const int MaxLines = 12;
        private const int MaxLineLength = 60;

        private readonly IRenderer Renderer;
        private readonly IFont Font;
        private IDialog currentDialog = new CompletedDialog();
        private DialogPage[] pages;
        private int currentPage;

        public bool HasActiveDialog => currentDialog?.Completed == false;

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
            pages = ProcessDialogText(currentDialog.Text);
            currentPage = 0;
            resetFrameCounter = 30;
        }

        private int resetFrameCounter = 0;
        public void Update(FrameCounter counter, InputMask[] inputMasks)
        {
            if (currentDialog.Completed)
            {
                resetFrameCounter = 0;
                return;
            }

            if (resetFrameCounter > 0)
            {
                resetFrameCounter--;
                inputMasks[0].Reset();
                return;
            }
            if (inputMasks[0].Input.Select)
            {
                PageDialog();
                resetFrameCounter = 10;
            }
        }

        private void PageDialog()
        {
            if (currentPage == pages.Length - 1)
            {
                CloseDialog();
            }
            else
            {
                currentPage++;
            }
        }

        private void CloseDialog()
        {
            currentDialog = new CompletedDialog();
            currentPage = 0;
        }

        public void Render(FrameCounter counter)
        {
            if (currentDialog.Completed)
            {
                return;
            }
            
            // Align center vertically
            var viewport = new Vector2(Renderer.GetDisplaySize());
            var textBox = new Vector2(MaxLineLength * CharacterWidth, -MaxLines * TextHeight);
            var position = (viewport - textBox) / 2f;

            Renderer.RenderRectangle(viewport / 2f - textBox / 2f, textBox, Color.White);

            // Render Lines of text
            foreach (var line in pages[currentPage].Lines)
            {
                Renderer.RenderText(Font, position, line, Color.Green, 0f, Vector2.Zero, new Vector2(3f, 3f));
                position -= new Vector2(0, TextHeight);
            }
        }

        private DialogPage[] ProcessDialogText(string text)
        {
            var dialogPages = new List<DialogPage>();
            var blocks = text.Split(new[] { "===\r\n"} , StringSplitOptions.RemoveEmptyEntries);
            foreach (var block in blocks)
            {
                var dialogLines = WrapText(block);

                // Page per max number of Lines
                var processedLines = 0;
                while (processedLines < dialogLines.Length)
                {
                    var pageSize = Math.Min(MaxLines, dialogLines.Length - processedLines);
                    dialogPages.Add(new DialogPage(dialogLines.Skip(processedLines).Take(pageSize)));
                    processedLines += pageSize;
                }
            }

            return dialogPages.ToArray();
        }

        private string[] WrapText(string text)
        {
            var lines = new List<string>();

            // Split on line breaks
            var paragraphs = text.Split(new[] { "\r\n" }, StringSplitOptions.None);

            foreach (var paragraph in paragraphs)
            {
                // Wrap to 80 character Lines but do not break words unless we have to
                var tokens = paragraph.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Build Lines
                var currentLine = "";
                foreach (var token in tokens)
                {
                    if (currentLine.Length + token.Length + 1 > MaxLineLength)
                    {
                        lines.Add(currentLine);
                        currentLine = "";
                    }

                    // Add the token to the line.
                    currentLine += (currentLine.Length == 0 ? "" : " ") + token;
                }
                lines.Add(currentLine);
            }

            return lines.ToArray();
        }

    }
}