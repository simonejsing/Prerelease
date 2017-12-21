using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CraftingGame.Physics;
using Contracts;
using VectorMath;
using CraftingGame.Items;
using CraftingGame.Actions;
using CraftingGame;
using FluentAssertions;

namespace MainGame.UnitTests
{
    [TestClass]
    public class ActionTests
    {
        [TestMethod]
        public void CollectableItemIsNotCollectedIfPlayerInventoryIsFull()
        {
            var action = new CollectAction();
            var player = new PlayerObject(null, null, new Plane(0), Vector2.Zero, Vector2.Zero, "", Color.Red);
            FillInventory(player);

            var item = ItemFactory.Create(nameof(BlockOfRock));
            var itemObject = new ItemObject(null, new Plane(0), Vector2.Zero, Vector2.Zero, item);
            action.Invoke(itemObject, itemObject, player);
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
