using CraftingGame.Items;
using CraftingGame.Physics;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;

namespace MainGame.UnitTests
{
    [TestClass]
    public class TerrainTargetTests
    {
        [TestMethod]
        public void TargetFindsBlockToTheRight()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0R
RRR");
            harness.StartGame();
            harness.Player.LookDirection = new Vector2(1, 0);
            var target = TargetNotFreeTerrain(harness);
            target.Update();
            target.Coordinate.Should().Be(new Coordinate(1, 0));
        }

        [TestMethod]
        public void TargetSticksWhenPlayerHasNoLookDirection()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0R
RRR");
            harness.StartGame();
            harness.Player.LookDirection = new Vector2(1, 0);
            var target = TargetNotFreeTerrain(harness);
            target.Update();
            harness.Player.LookDirection = new Vector2(0, 0);
            target.Update();
            target.Coordinate.Should().Be(new Coordinate(1, 0));
        }

        [TestMethod]
        public void TargetIsDroppedWhenPlayerMovesTooFarAway()
        {
            var harness = GameHarness.CreateFromMap(
@"...
.0R
RRR");
            harness.StartGame();
            harness.Player.LookDirection = new Vector2(1, 0);
            var target = TargetNotFreeTerrain(harness);
            target.Update();
            target.Coordinate.Should().Be(new Coordinate(1, 0));
            harness.Player.Position = new Vector2(100, 100);
            target.Update();
            target.Coordinate.Should().BeNull();
        }

        private static TerrainTarget TargetNotFreeTerrain(GameHarness harness)
        {
            var target = new TerrainTarget(harness.Game.State, t => t != TerrainType.Free);
            target.Equip(harness.Player);
            return target;
        }
    }
}
