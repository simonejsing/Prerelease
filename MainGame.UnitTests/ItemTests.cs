using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using CraftingGame;
using CraftingGame.Items;

namespace MainGame.UnitTests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        public void CanCreateAllItems()
        {
            var itemTypes = GetAllItemTypes();
            itemTypes.Length.Should().BeGreaterThan(0);

            foreach(var type in itemTypes)
            {
                ItemFactory.CreateItem(type.Name).Should().NotBeNull($"item type {type} should be constructable. Did you miss adding to the factory?");
            }
        }

        [TestMethod]
        public void CanCreateItemsFromTerrainTypes()
        {
            foreach (TerrainType type in Enum.GetValues(typeof(TerrainType)))
            {
                switch (type)
                {
                    case TerrainType.NotGenerated:
                    case TerrainType.Free:
                    case TerrainType.Sea:
                    case TerrainType.Bedrock:
                        break;
                    default:
                        ItemFactory.ItemFromTerrain(type).Should().NotBeNull($"terrain type {type} should be minable");
                        break;
                }
            }
        }

        private Type[] GetAllItemTypes()
        {
            var assembly = typeof(ItemFactory).Assembly;
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, "CraftingGame.Items", StringComparison.Ordinal) && !t.IsAbstract).ToArray();
        }
    }
}
