using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CraftingGame.Physics;
using CraftingGame.Physics.Items;
using VectorMath;
using FluentAssertions;

namespace MainGame.UnitTests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void CanAddAnItemToInventory()
        {
            var inventory = new Inventory();
            var block = new BlockOfRock();
            inventory.Count(block.Name).Should().Be(0);
            inventory.Add(block.Name);
            inventory.Count(block.Name).Should().Be(1);
        }

        [TestMethod]
        public void CanTakeAnItemFromInventory()
        {
            var inventory = new Inventory();
            var block = new BlockOfRock();
            inventory.Add(block.Name);
            inventory.CanTake(block.Name).Should().BeTrue();
            var item = inventory.Take(block.Name);
            inventory.Count(block.Name).Should().Be(0);
            inventory.CanTake(block.Name).Should().BeFalse();
        }
    }
}
