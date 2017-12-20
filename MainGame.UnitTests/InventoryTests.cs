using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CraftingGame.Physics;
using CraftingGame.Items;
using VectorMath;
using FluentAssertions;
using Contracts;

namespace MainGame.UnitTests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void CanAddAnItemToInventory()
        {
            var inventory = new Inventory();
            var block = nameof(BlockOfRock);
            inventory.Count(block).Should().Be(0);
            inventory.Add(block);
            inventory.Count(block).Should().Be(1);
        }

        [TestMethod]
        public void CanTakeAnItemFromInventory()
        {
            var inventory = new Inventory();
            var block = nameof(BlockOfRock);
            inventory.Add(block);
            inventory.CanTake(block).Should().BeTrue();
            var item = inventory.Take(block);
            inventory.Count(block).Should().Be(0);
            inventory.CanTake(block).Should().BeFalse();
        }

        [TestMethod]
        public void CanConsumeItemFromInventory()
        {
            var player = new PlayerObject(null, null, new Plane(0), Vector2.Zero, Vector2.Zero, "", Color.Red);
            var inventory = new Inventory();
            var flower = nameof(ConsumableFlower);
            inventory.Add(flower);
            inventory.CanTake(flower).Should().BeTrue();
            inventory.Consume(player, flower);
            inventory.Count(flower).Should().Be(0);
            inventory.CanTake(flower).Should().BeFalse();
            player.Dead.Should().BeTrue();
        }
    }
}
