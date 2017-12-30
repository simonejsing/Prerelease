using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CraftingGame.Physics;
using Contracts;
using VectorMath;
using CraftingGame.Items;
using CraftingGame.Actions;
using CraftingGame;
using FluentAssertions;
using CraftingGame.Items.Creatable;

namespace MainGame.UnitTests
{
    [TestClass]
    public class ActionTests
    {
        [TestMethod]
        public void CollectableItemIsNotCollectedIfPlayerInventoryIsFull()
        {
            var player = new PlayerObject(null);
            FillInventory(player);

            var item = ItemFactory.CreateItem(nameof(BlockOfRock));
            var itemObject = new ItemObject(null, item);
            CollectAction.Invoke(itemObject, itemObject, player);
            itemObject.Collected.Should().BeFalse();
        }

        private static void FillInventory(PlayerObject player)
        {
            while (!player.Inventory.Full)
            {
                player.Inventory.Add(nameof(BlockOfRock));
            }
        }
    }
}
