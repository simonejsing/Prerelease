using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using Contracts;
using System.IO;
using FluentAssertions;
using VectorMath;
using CraftingGame.Physics;
using System.Linq;
using Serialization;

namespace MainGame.UnitTests
{
    [TestClass]
    public class GameStateTests
    {
        /*[TestMethod]
        public void TestSerializingDictionary()
        {
            var state = new SerializableState();
            state.AddObject("player", 15);

            //using (var stream = new MemoryStream())
            using (var stream = File.OpenWrite("state.json"))
            {
                state.Serialize(stream);
            }
            using(var stream = File.OpenRead("state.json"))
            {
                var newState = SerializableState.FromStream(stream);
                newState.State["player"].Should().Be(15);
            }
        }*/

        [TestMethod]
        public void CanPersistPlayerObject()
        {
            var state = new GameState(new ActionQueue());
            var player = new PlayerObject(null, Guid.NewGuid(), "player1", new Plane(5), new Vector2(100, 100), new Vector2(30, 30), "Chicken", Color.Red);
            state.AddPlayer(player);
            using (var stream = new MemoryStream())
            {
                state.SaveToStream(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var loadState = new GameState(new ActionQueue());
                loadState.LoadFromStream(stream);
                var player1 = loadState.Players.First();
                player1.PlayerBinding.Should().Be("player1");
                player1.Plane.W.Should().Be(5);
                player1.Position.Should().Be(new Vector2(100, 100));
                player1.Size.Should().Be(new Vector2(30, 30));
                player1.SpriteBinding.Path.Should().Be("Chicken");
                player1.Color.Should().Be(Color.Red);
            }
        }

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
                stream.Seek(0, SeekOrigin.Begin);

                // Load game
                var loadedHarness = constructor();
                loadedHarness.LoadGame(stream);

                verifier(loadedHarness);
            }
        }
    }
}
