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
        public void CannotAddMoreThanCapacityToInventory()
        {
            const int capacity = 10;
            var inventory = new Inventory(capacity);
            var block = nameof(BlockOfRock);
            for(var i = 0; i < capacity; i++)
            {
                inventory.Add(block).Should().BeTrue();
            }
            inventory.Count(block).Should().Be(capacity);
            inventory.Add(block).Should().BeFalse();
        }

        [TestMethod]
        public void CanAddAnItemToInventory()
        {
            var inventory = new Inventory(10);
            var block = nameof(BlockOfRock);
            inventory.Count(block).Should().Be(0);
            inventory.Add(block).Should().BeTrue();
            inventory.Count(block).Should().Be(1);
        }

        [TestMethod]
        public void CanTakeAnItemFromInventory()
        {
            var inventory = new Inventory(10);
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
            var player = new PlayerObject(null, Guid.NewGuid(), null, new Plane(0), Vector2.Zero, Vector2.Zero, "", Color.Red);
            var inventory = new Inventory(10);
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
