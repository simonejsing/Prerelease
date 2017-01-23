using System.Collections.Generic;

namespace Contracts
{
    public interface IDialog
    {
        ICharacter SourceCharacter { get; }
        ICharacter TargetCharacter { get; }
        IEnumerable<IDialogOption> Options { get; }
        string Text { get; }
        bool Completed { get; }

        IDialog ChooseOption(IDialogOption option);
    }

    public interface IDialogOption
    {
        string Text { get; }
        GameAction Action { get; }
    }
}