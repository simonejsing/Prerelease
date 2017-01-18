using System.Collections.Generic;

namespace Prerelease.Main
{
    public enum ActionType { Quit };

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