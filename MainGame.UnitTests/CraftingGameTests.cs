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
            harness.StartGame();
            harness.Update(0.1f);
            harness.Render(0.1f);
        }

        [TestMethod]
        public void RendersTerrainBlockWhenInsideView()
        {
            var blockCoord = new Coordinate(0, 0);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.StartGame();
            harness.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void RendersTerrainBlockWhenTranslated()
        {
            var blockCoord = new Coordinate(2, 1);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.StartGame();
            harness.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void DoesNotRenderTerrainBlockWhenOutOfView()
        {
            var blockCoord = new Coordinate(2, 1);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.StartGame();
            harness.Game.View.Translate(GameHarness.DefaultViewPort * 10);
            harness.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord, Times.Never());
        }

        [TestMethod]
        public void RendersTerrainBlockWhenScaled()
        {
            var blockCoord = new Coordinate(0, 0);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.StartGame();
            // Zoom out by 10%
            harness.Game.View.Scale(1.1f);
            harness.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void DoesNotRenderTerrainBlockWhenZoomedIn()
        {
            var blockCoord = new Coordinate(2, 2);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.StartGame();
            // Zoom in by 100%
            harness.Game.View.Scale(0.5f);
            harness.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord, Times.Never());
        }

        [TestMethod]
        public void RendersTerrainBlockWhenViewIsTranslated()
        {
            var blockCoord = new Coordinate(0, 0);
            var harness = GameHarness.CreateSingleBlockGame(blockCoord, TerrainType.Rock);
            harness.StartGame();
            harness.Game.View.Translate(new Vector2(10, 20));
            harness.Render(0.1f);
            harness.VerifyBlockRendered(blockCoord);
        }

        [TestMethod]
        public void HandlesPlayerMovingRight()
        {
            var harness = GameHarness.CreateEmptyGame();
            harness.StartGame();
            var initialX = harness.Player.Position.X;
            harness.Input(right: true);
            harness.Update(0.1f);
            harness.Player.Position.X.Should().BeGreaterThan(initialX);
        }

        [TestMethod]
        public void PlayerSpawnsOnTerrain()
        {
            var harness = GameHarness.CreateSolidBlockGame(TerrainType.Rock);
            harness.StartGame();
            var playerCoord = harness.PlayerCoordinate;
            playerCoord.U.Should().Be(0);
            playerCoord.V.Should().Be(0);
        }
    }
}
