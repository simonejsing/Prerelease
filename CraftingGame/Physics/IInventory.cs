using CraftingGame.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingGame.Physics
{
    public interface IInventory
    {
        int Capacity { get; }
        int TotalCount { get; }
        bool Full { get; }

        bool Add(string item);
        int Count(string name);
        bool CanTake(string name);
        StackableItemBase Take(string name);
        void Consume(PlayerObject player, string name);
    }
}
