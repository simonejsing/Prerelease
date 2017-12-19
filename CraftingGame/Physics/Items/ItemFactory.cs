using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace CraftingGame.Physics.Items
{
    internal static class ItemFactory
    {
        public static IDictionary<string, Func<Item>> factories = new Dictionary<string, Func<Item>>();

        public static void Register(string name, Func<Item> factory)
        {
            var key = name.ToLower();
            factories.Add(key, factory);
        }

        public static Item Create(string name)
        {
            var key = name.ToLower();
            if (!factories.ContainsKey(key))
            {
                throw new InvalidOperationException($"No factory function registered for item of type {key}.");
            }
            return factories[key]();
        }

        internal static Item FromTerrain(TerrainType type)
        {
            switch(type)
            {
                case TerrainType.Rock:
                    return new BlockOfRock();
                default:
                    return null;
            }
        }
    }
}
