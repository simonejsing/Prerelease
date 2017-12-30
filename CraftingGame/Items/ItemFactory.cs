using CraftingGame.Items;
using CraftingGame.Items.Creatable;
using CraftingGame.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace CraftingGame.Items
{
    public class ItemFactory
    {
        private readonly GameState state;

        public static IDictionary<string, Func<StackableItemBase>> itemFactories = new Dictionary<string, Func<StackableItemBase>>();

        internal ItemFactory(GameState state)
        {
            this.state = state;
        }

        static ItemFactory()
        {
            RegisterItem(nameof(NoopItem), () => new NoopItem());
            RegisterItem(nameof(BlockOfDirt), () => new BlockOfDirt());
            RegisterItem(nameof(BlockOfRock), () => new BlockOfRock());
            RegisterItem(nameof(ConsumableFlower), () => new ConsumableFlower());
        }

        private static void RegisterItem(string name, Func<StackableItemBase> factory)
        {
            var key = name.ToLower();
            itemFactories.Add(key, factory);
        }

        /*public IEquipableItem CreateEquipableItem(string name)
        {
        }*/

        public IEquipableItem EquipableItemFromTerrain(TerrainType type)
        {
            switch (type)
            {
                case TerrainType.Dirt:
                case TerrainType.Rock:
                    return new PlacableTerrainBlock(state, type);
                default:
                    return null;
            }
        }

        public static StackableItemBase CreateItem(string name)
        {
            var key = name.ToLower();
            if (!itemFactories.ContainsKey(key))
            {
                throw new InvalidOperationException($"No factory function registered for item of type {key}.");
            }
            return itemFactories[key]();
        }

        public static StackableItemBase ItemFromTerrain(TerrainType type)
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
