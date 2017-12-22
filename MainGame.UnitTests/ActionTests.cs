﻿using System;
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
            var player = new PlayerObject(null);
            FillInventory(player);

            var item = ItemFactory.Create(nameof(BlockOfRock));
            var itemObject = new ItemObject(null, item);
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
