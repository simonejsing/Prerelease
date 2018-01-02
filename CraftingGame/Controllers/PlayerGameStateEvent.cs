using CraftingGame.Physics;
using System;

namespace CraftingGame.Controllers
{
    public class PlayerGameStateEvent : EventArgs
    {
        public PlayerObject Player;

        public PlayerGameStateEvent(PlayerObject player)
        {
            Player = player;
        }
    }
}