using CraftingGame.Physics.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingGame.Physics
{
    public interface IInventory
    {
        int TotalCount { get; }

        void Add(string item);
        int Count(string name);
        bool CanTake(string name);
        ItemBase Take(string name);
        void Consume(PlayerObject player, string name);
    }
}
