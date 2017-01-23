using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace HubSceene
{
    class Dialog : IDialog
    {
        private readonly ActionQueue actionQueue;
        protected IList<IDialogOption> options = new List<IDialogOption>();

        public ICharacter SourceCharacter { get; private set; }
        public ICharacter TargetCharacter { get; private set; }
        public IEnumerable<IDialogOption> Options => options;
        public string Text { get; }
        public bool Completed => false;

        public Dialog(ActionQueue actionQueue, string text)
        {
            this.actionQueue = actionQueue;
            this.Text = text;
        }

        public void AddDialogOption(IDialogOption dialogOption)
        {
            options.Add(dialogOption);
        }

        public IDialog ChooseOption(IDialogOption option)
        {
            // Dispatch action to game.
            actionQueue.Enqueue(option.Action);

            return Dialog.Done;
        }

        public static string ReadEmbeddedDialog(Type assemblyType, string dialogName)
        {
            var assembly = assemblyType.GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream(dialogName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static IDialog Done => new CompletedDialog();
    }
}
