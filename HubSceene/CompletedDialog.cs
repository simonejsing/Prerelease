using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;

namespace HubSceene
{
    public class CompletedDialog : IDialog
    {
        public ICharacter SourceCharacter => new NoOne();
        public ICharacter TargetCharacter => new NoOne();
        public IEnumerable<IDialogOption> Options => Enumerable.Empty<IDialogOption>();
        public string Text => string.Empty;
        public bool Completed => true;

        public IDialog ChooseOption(IDialogOption option)
        {
            throw new Exception("Cannot choose option in dialog when it is completed.");
        }
    }
}