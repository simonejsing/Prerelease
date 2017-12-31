using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using Contracts;
using System.IO;
using FluentAssertions;
using VectorMath;
using CraftingGame.Physics;
using System.Linq;
using CraftingGame.State;
using Serialization;
using CraftingGame;
using System.Collections.Generic;
using CraftingGame.Items;

namespace MainGame.UnitTests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void TestInMemoryStreamProviderPersistsContent()
        {
            const string filename = "file";
            const string content = "some content in the file";

            var provider = new InMemoryStreamProvider();
            provider.FileExists(filename).Should().BeFalse();
            using (var writer = new StreamWriter(provider.WriteFile(filename)))
            {
                writer.Write(content);
            }

            provider.FileExists(filename).Should().BeTrue();
            using (var reader = new StreamReader(provider.ReadFile(filename)))
            {
                reader.ReadToEnd().Should().Be(content);
            }
        }

        [TestMethod]
        public void CanPersistPlayerObject()
        {
            var player = new PlayerObject(null)
            {
                Id = Guid.NewGuid(),
                PlayerBinding = "player1",
                SpriteBinding = new ObjectBinding<ISprite>("Chicken"),
                Plane = new Plane(5),
                Position = new Vector2(100, 100),
                Velocity = new Vector2(-20, 15),
                Size = new Vector2(30, 30),
                Color = Color.Red,
            };
            var builder = new StatefulObjectBuilder();
            player.ExtractState(builder);

            var newPlayer = PlayerObject.FromState(new StatefulObject(null, player.Id, builder.State));
            newPlayer.Id.Should().Be(player.Id);
            newPlayer.PlayerBinding.Should().Be("player1");
            newPlayer.Plane.W.Should().Be(5);
            newPlayer.Position.Should().Be(new Vector2(100, 100));
            newPlayer.Velocity.Should().Be(new Vector2(-20, 15));
            newPlayer.Size.Should().Be(new Vector2(30, 30));
            newPlayer.SpriteBinding.Path.Should().Be("Chicken");
            newPlayer.Color.Should().Be(Color.Red);
        }

        [TestMethod]
        public void CanPersistTerrainConfiguration()
        {
            var factory = new TerrainFactory(10, 10, 8, seed: 512);
            var grid = new Grid(30, 30);
            var state = new GameState(null, grid, factory)
            {
                Terrain = new CachedTerrainGenerator(factory.Create())
            };

            using (var stream = new MemoryStream())
            {
                state.SaveToStream(stream);
                stream.Seek(0, SeekOrigin.Begin);

                // Load game state
                var newFactory = new TerrainFactory(100, 100, 80, seed: 0);
                var newState = new GameState(null, grid, newFactory);
                newState.LoadFromStream(stream);
                newState.Terrain.MaxDepth.Should().Be(10);
                newState.Terrain.MaxHeight.Should().Be(10);
                newState.Terrain.SeaLevel.Should().Be(8);
                newState.Terrain.Seed.Should().Be(512);
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

        [TestMethod]
        public void GameStateLimitsNumberOfCollecableObjectsToTwenty()
        {
            var state = CreateDefaultState();
            for(var i = 0; i < 21; i++)
            {
                var item = ItemFactory.ItemFromTerrain(TerrainType.Dirt);
                var itemObject = new ItemObject(state.ActionQueue, item);
                state.ActiveLevel.AddCollectableObjects(itemObject);
            }
            state.ActiveLevel.CollectableObjects.Count().Should().Be(20);
        }

        private static GameState CreateDefaultState()
        {
            var factory = new TerrainFactory(100, 100, 80);
            var state = new GameState(new ActionQueue(), new Grid(30, 30), factory);
            state.AddLevel(new LevelState("level", Vector2.Zero));
            state.SetActiveLevel("level");
            return state;
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
