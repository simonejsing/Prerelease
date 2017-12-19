using CraftingGame.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace CraftingGame
{
    public static class ItemFactory
    {
        public static IDictionary<string, Func<ItemBase>> factories = new Dictionary<string, Func<ItemBase>>();

        static ItemFactory()
        {
            Register(nameof(NoopItem), () => new NoopItem());
            Register(nameof(BlockOfDirt), () => new BlockOfDirt());
            Register(nameof(BlockOfRock), () => new BlockOfRock());
            Register(nameof(ConsumableFlower), () => new ConsumableFlower());
        }

        private static void Register(string name, Func<ItemBase> factory)
        {
            var key = name.ToLower();
            factories.Add(key, factory);
        }

        public static ItemBase Create(string name)
        {
            var key = name.ToLower();
            if (!factories.ContainsKey(key))
            {
                throw new InvalidOperationException($"No factory function registered for item of type {key}.");
            }
            return factories[key]();
        }

        public static ItemBase FromTerrain(TerrainType type)
        {
            switch(type)
            {
                case TerrainType.Dirt:
                    return new BlockOfDirt();
                case TerrainType.Rock:
                    return new BlockOfRock();
                default:
                    return null;
            }
        }
    }
}
