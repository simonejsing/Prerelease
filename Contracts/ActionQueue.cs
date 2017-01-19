using System.Collections.Generic;

namespace Contracts
{
    public enum ActionType {
        Quit,
        NewGame
    };

    public struct GameAction
    {
        public ActionType Type;

        public GameAction(ActionType type)
        {
            this.Type = type;
        }    
    }

    public class ActionQueue : Queue<GameAction>
    {
    }
}