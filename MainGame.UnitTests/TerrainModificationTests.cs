using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using FluentAssertions;
using System.Linq;
using Contracts;

namespace MainGame.UnitTests
{
    [TestClass]
    public class TerrainModificationTests
    {
        [TestMethod]
        public void ShouldBeAbleToDigAndCollectDroppedBlock()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(right: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"...
.0.
RR.");

            // Digging into terrain should drop an item
            var dropCoord = new Coordinate(1, -1);
            var drop = harness.Game.State.ActiveLevel.CollectableObjects.First();
            harness.Game.Grid.PointToGridCoordinate(drop.Center).Should().Be(dropCoord);
            drop.Should().NotBeNull();
            drop.Item.Name.Should().Be("BlockOfRock");

            // Teleport player ontop of item and collect the item
            harness.Player.Position = harness.Game.Grid.GridCoordinateToPoint(dropCoord);
            harness.Input();
            harness.Game.Update(0.1f);

            // Verify inventory contains dropped item
            harness.Player.Inventory.Count("BlockOfRock").Should().Be(1);
        }

        [TestMethod]
        public void ShouldNotModifyTerrainIfPlayerIsNotLooking()
        {
            var harness = GameHarness.CreateFromMap(
@"RRR
R0R
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"RRR
R0R
RRR");
        }

        [TestMethod]
        public void PlayerCanDigRight()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0R
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(right: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"...
.0.
RRR");
        }

        [TestMethod]
        public void PlayerCanDigLeft()
        {
            var harness = GameHarness.CreateFromMap(
@"...
R0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(left: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"...
.0.
RRR");
        }

        [TestMethod]
        public void PlayerCanDigDownRight()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(down: true, right: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"...
.0.
RR.");
        }

        [TestMethod]
        public void PlayerCanDigDownLeft()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(down: true, left: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"...
.0.
.RR");
        }

        [TestMethod]
        public void PlayerCanDigStraightDown()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(down: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"...
.0.
R.R");
        }

        [TestMethod]
        public void PlayerCanDigStraightUp()
        {
            var harness = GameHarness.CreateFromMap(
@"RRR
.0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(up: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"R.R
.0.
RRR");
        }

        [TestMethod]
        public void PlayerCanDigStraightUpLeft()
        {
            var harness = GameHarness.CreateFromMap(
@"RRR
.0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(up: true, left: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@".RR
.0.
RRR");
        }

        [TestMethod]
        public void PlayerCanDigStraightUpRight()
        {
            var harness = GameHarness.CreateFromMap(
@"RRR
.0.
RRR");
            harness.StartGame();
            harness.Input();
            harness.Game.Update(0.1f);
            harness.Input(up: true, right: true, attack: true);
            harness.Game.Update(0.1f);
            harness.VerifyTerrain(
@"RR.
.0.
RRR");
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

        [TestMethod]
        public void TestVerifyTerrain()
        {
            const string terrainMap =
@"...
.0.
RRR";
            var harness = GameHarness.CreateFromMap(terrainMap);
            harness.StartGame();
            harness.VerifyTerrain(terrainMap);
        }
    }
}
