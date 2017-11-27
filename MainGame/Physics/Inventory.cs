using System.Collections.Generic;

namespace Prerelease.Main.Physics
{
    internal class Inventory : IInventory
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();
         
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
    }
}