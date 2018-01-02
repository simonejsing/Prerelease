using System;
using System.Collections.Generic;
using System.Text;

namespace CraftingGame.State.Upgrade
{
    internal class GameStateUpgradeV2 : IGameStateUpgradeRule
    {
        public int FromVersion => 1;

        public int ToVersion => 2;

        public void Upgrade(GameState gameState)
        {
            foreach(var player in gameState.KnownPlayers)
            {
                if(player.Inventory.Capacity == 100)
                {
                    player.Inventory.Capacity = 500;
                }
            }
        }
    }
}
