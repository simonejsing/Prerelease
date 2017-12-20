using System;
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
            harness.Input(right: true);
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
            var harness = GameHarness.CreateFromMap(
@"...
.0.
RRR");
            var digCoord = new Coordinate(1, -1);
            harness.ReadTerrainType(digCoord).Should().Be(TerrainType.Rock);
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(attack: true);
            harness.Game.Update(0.1f);
            harness.ReadTerrainType(digCoord).Should().Be(TerrainType.Free);

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

        [TestMethod]
        public void PlayerCanDigStraightDown()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0.
RRR");
            harness.ReadTerrainType(0, -1).Should().Be(TerrainType.Rock);
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(down: true, attack: true);
            harness.Game.Update(0.1f);
            harness.ReadTerrainType(0, -1).Should().Be(TerrainType.Free);
        }

        [TestMethod]
        public void TestTerrainMap()
        {
            var plane = new Plane(0);
            var terrain = new TerrainStub(
@"RRR
.0.
RRR");
            terrain[new Coordinate(-1, 1), plane].Type.Should().Be(TerrainType.Rock);
            terrain[new Coordinate(0, 1), plane].Type.Should().Be(TerrainType.Rock);
            terrain[new Coordinate(1, 1), plane].Type.Should().Be(TerrainType.Rock);
            terrain[new Coordinate(-1, 0), plane].Type.Should().Be(TerrainType.Free);
            terrain[new Coordinate(0, 0), plane].Type.Should().Be(TerrainType.Free);
            terrain[new Coordinate(1, 0), plane].Type.Should().Be(TerrainType.Free);
            terrain[new Coordinate(-1, -1), plane].Type.Should().Be(TerrainType.Rock);
            terrain[new Coordinate(0, -1), plane].Type.Should().Be(TerrainType.Rock);
            terrain[new Coordinate(1, -1), plane].Type.Should().Be(TerrainType.Rock);
        }

        private static Coordinate PlayerCoordinate(GameHarness harness)
        {
            return harness.Game.Grid.PointToGridCoordinate(harness.Player.Center);
        }
    }
}
