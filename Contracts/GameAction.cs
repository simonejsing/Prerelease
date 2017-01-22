namespace Contracts
{
    public enum ActionType
    {
        Noop,
        Quit,
        NewGame,
        DialogOption,
        AdvanceDialog
    };

    public struct GameAction
    {
        public ActionType Type;

        public GameAction(ActionType type)
        {
            this.Type = type;
        }    
    }
}