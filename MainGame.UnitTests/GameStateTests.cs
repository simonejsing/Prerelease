using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using Contracts;
using System.IO;
using FluentAssertions;
using VectorMath;

namespace MainGame.UnitTests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void PlayerPositionIsPersistedWhenLoadingGameState()
        {
            var newPosition = new Vector2(100, 100);
            TestGameStateModification(
                () => GameHarness.CreateSolidBlockGame(TerrainType.Rock),
                h => h.Player.Position = newPosition,
                h => h.Player.Position.Should().Be(newPosition));
        }

        [TestMethod]
        public void TerrainModificationPersistsWhenLoadingGameState()
        {
            var modificationCoord = new Coordinate(0, -1);
            TestGameStateModification(
                () => GameHarness.CreateSolidBlockGame(TerrainType.Rock),
                h => h.Game.Terrain.Destroy(modificationCoord, new Plane(0)),
                h => h.ReadTerrainType(modificationCoord).Should().Be(TerrainType.Free));
        }

        private static void TestGameStateModification(Func<GameHarness> constructor, Action<GameHarness> modifier, Action<GameHarness> verifier)
        {
            var harness = constructor();
            harness.StartGame();
            modifier(harness);

            using (var stream = new MemoryStream())
            {
                harness.Game.State.SaveToStream(stream);

                // Load game
                var loadedHarness = constructor();
                loadedHarness.LoadGame(stream);

                verifier(loadedHarness);
            }
        }
    }
}
