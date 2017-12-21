using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using Contracts;
using System.IO;
using FluentAssertions;

namespace MainGame.UnitTests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void TerrainModificationPersistsWhenLoadingGameState()
        {
            /*var harness = GameHarness.CreateFromMap(
@"0..
RRR
RRR");
            harness.StartGame();
            var modificationCoord = new Coordinate(0, -1);

            harness.Game.Terrain.Destroy(modificationCoord, new Plane(0));
            using(var stream = new MemoryStream())
            {
                harness.Game.State.SaveToStream(stream);

                // Load game
                var harness2 = GameHarness.CreateFromMap(
@"0..
RRR
RRR");
                harness2.LoadGame(stream);
                harness2.ReadTerrainType(modificationCoord).Should().Be(TerrainType.Free);
            }*/
            Assert.Fail();
        }
    }
}
