using CraftingGame.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CraftingGame.Physics
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();

        public int Capacity { get; }
        public int TotalCount => items.Values.Sum();
        public bool Full => Capacity == TotalCount;

        public Inventory(int capacity)
        {
            this.Capacity = capacity;
        }

        public bool Add(string item)
        {
            if (Full)
                return false;

            if (!items.ContainsKey(item))
            {
                items.Add(item, 1);
            }
            else
            {
                items[item]++;
            }

            return true;
        }

        public int Count(string name)
        {
            return items.ContainsKey(name) ? items[name] : 0;
        }

        public bool CanTake(string name)
        {
            return items.ContainsKey(name) && items[name] > 0;
        }

        public StackableItemBase Take(string name)
        {
            if (!CanTake(name))
                return new NoopItem();

            items[name]--;
            return ItemFactory.Create(name);
        }

        public void Consume(PlayerObject player, string name)
        {
            if (!CanTake(name))
                return;

            var item = ItemFactory.Create(name);
            if(item.Consumable)
            {
                items[name]--;
                item.Consume(player);
            }
        }
    }
}