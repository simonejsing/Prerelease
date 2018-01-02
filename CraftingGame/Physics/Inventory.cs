using Contracts;
using CraftingGame.Items;
using CraftingGame.Items.Creatable;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Terrain;

namespace CraftingGame.Physics
{
    public class Inventory : IInventory, IStatefulEntity
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();

        public int Capacity { get; set; }
        public int TotalCount => items.Values.Sum();
        public bool Full => Capacity == TotalCount;

        public Guid Id { get; set; }

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

        public int Count(TerrainType type)
        {
            var name = ItemFactory.ItemFromTerrain(type).Name;
            return Count(name);
        }

        public int Count(string name)
        {
            return items.ContainsKey(name) ? items[name] : 0;
        }

        public bool CanTake(TerrainType type)
        {
            var name = ItemFactory.ItemFromTerrain(type).Name;
            return CanTake(name);
        }

        public bool CanTake(string name)
        {
            return items.ContainsKey(name) && items[name] > 0;
        }

        public StackableItemBase Take(TerrainType type)
        {
            var name = ItemFactory.ItemFromTerrain(type).Name;
            return Take(name);
        }

        public StackableItemBase Take(string name)
        {
            if (!CanTake(name))
                return new NoopItem();

            items[name]--;
            return ItemFactory.CreateItem(name);
        }

        public void Consume(PlayerObject player, string name)
        {
            if (!CanTake(name))
                return;

            var item = ItemFactory.CreateItem(name);
            if(item.Consumable)
            {
                items[name]--;
                item.Consume(player);
            }
        }

        public void ExtractState(StatefulObjectBuilder builder)
        {
            builder.Add("i.c", Capacity);
            builder.Add("i.si", items);
        }

        internal static Inventory FromState(StatefulObject state)
        {
            var capacity = state.SafeReadValue("i.c", 100);
            var inventory = new Inventory(capacity);

            // Load items
            var items = state.SafeReadDictionary("i.si");
            foreach(var itemName in items.Keys)
            {
                inventory.items[itemName] = items.SafeReadValue(itemName, 0);
            }

            return inventory;
        }
    }
}