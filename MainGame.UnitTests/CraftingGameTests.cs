﻿using System;
using System.Linq;
using Contracts;
using CraftingGame;
using CraftingGame.Physics;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Terrain;
using VectorMath;

namespace MainGame.UnitTests
{
    [TestClass]
    public class CraftingGameTests
    {
        [TestMethod]
        public void CanInstantiateGameAndRunUpdateAndRenderCycle()
        {
            var harness = GameHarness.CreateEmptyGame();
            harness.Game.Update(0.1f);
            harness.Game.Render(0.1f);
        }

        [TestMethod]
        public void RendersTerrainBlockWhenInsideView()
        {
            var blockCoord = new Coordinate(0, 0);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.Game.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void RendersTerrainBlockWhenTranslated()
        {
            var blockCoord = new Coordinate(2, 1);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.Game.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void DoesNotRenderTerrainBlockWhenOutOfView()
        {
            var blockCoord = new Coordinate(2, 1);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.Game.View.Translate(GameHarness.DefaultViewPort * 10);
            harness.Game.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord, Times.Never());
        }

        [TestMethod]
        public void RendersTerrainBlockWhenScaled()
        {
            var blockCoord = new Coordinate(0, 0);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            // Zoom out by 10%
            harness.Game.View.Scale(1.1f);
            harness.Game.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void DoesNotRenderTerrainBlockWhenZoomedIn()
        {
            var blockCoord = new Coordinate(2, 2);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            // Zoom in by 100%
            harness.Game.View.Scale(0.5f);
            harness.Game.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord, Times.Never());
        }

        [TestMethod]
        public void RendersTerrainBlockWhenViewIsTranslated()
        {
            var blockCoord = new Coordinate(0, 0);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.Game.View.Translate(new Vector2(10, 20));
            harness.Game.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void HandlesPlayerMovingRight()
        {
            var harness = GameHarness.CreateEmptyGame();
            var initialX = harness.Player.Position.X;
            harness.Input(moveRight: true);
            harness.Game.Update(0.1f);
            harness.Player.Position.X.Should().BeGreaterThan(initialX);
        }

        [TestMethod]
        public void PlayerSpawnsOnTerrain()
        {
            var harness = GameHarness.CreateSolidBlockGame(TerrainType.Rock);
            var playerCoord = harness.Game.Grid.PointToGridCoordinate(harness.Player.Center);
            playerCoord.U.Should().Be(0);
            playerCoord.V.Should().Be(0);
        }

        [TestMethod]
        public void PlayerCanDigIntoTerrain()
        {
            var plane = new Plane(0);
            var harness = GameHarness.CreateSolidBlockGame(TerrainType.Rock);
            var digCoord = DigCoordinate(harness, harness.Player);
            harness.Game.Terrain[digCoord, plane].Type.Should().Be(TerrainType.Rock);
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(attack: true);
            harness.Game.Update(0.1f);
            harness.Game.Terrain[digCoord, plane].Type.Should().Be(TerrainType.Free);

            // Digging into terrain should drop an item
            var drop = harness.Game.State.ActiveLevel.CollectableObjects.First();
            harness.Game.Grid.PointToGridCoordinate(drop.Center).Should().Be(digCoord);
            drop.Should().NotBeNull();
            drop.Item.Name.Should().Be("BlockOfRock");

            // Teleport player ontop of item and collect the item
            harness.Player.Position = harness.Game.Grid.GridCoordinateToPoint(digCoord);
            harness.Input();
            harness.Game.Update(0.1f);

            // Verify inventory contains dropped item
            harness.Player.Inventory.Count("BlockOfRock").Should().Be(1);
        }

        private static Coordinate DigCoordinate(GameHarness harness, PlayerObject player)
        {
            var playerCoord = harness.Game.Grid.PointToGridCoordinate(player.Center);
            return new Coordinate(playerCoord.U + Math.Sign(player.Facing.X), playerCoord.V - 1);
        }
    }
}
