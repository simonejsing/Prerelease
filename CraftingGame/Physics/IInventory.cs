using CraftingGame.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace CraftingGame.Physics
{
    public interface IInventory
    {
        int Capacity { get; set; }
        int TotalCount { get; }
        bool Full { get; }

        bool Add(string item);
        int Count(string name);
        int Count(TerrainType type);
        bool CanTake(string name);
        bool CanTake(TerrainType type);
        StackableItemBase Take(string name);
        StackableItemBase Take(TerrainType type);
        void Consume(PlayerObject player, string name);
    }
}
