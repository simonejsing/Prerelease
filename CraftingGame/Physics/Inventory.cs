﻿using CraftingGame.Physics.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CraftingGame.Physics
{
    public class Inventory : IInventory
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();

        public int TotalCount => items.Values.Sum();

        public void Add(string item)
        {
            if (!items.ContainsKey(item))
            {
                items.Add(item, 1);
            }
            else
            {
                items[item]++;
            }
        }

        public int Count(string name)
        {
            return items.ContainsKey(name) ? items[name] : 0;
        }

        public bool CanTake(string name)
        {
            return items.ContainsKey(name) && items[name] > 0;
        }

        public Item Take(string name)
        {
            if (!CanTake(name))
                return new NoopItem();

            items[name]--;
            return ItemFactory.Create(name);
        }
    }
}