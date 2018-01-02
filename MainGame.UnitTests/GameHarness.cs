using Contracts;
using CraftingGame;
using CraftingGame.Physics;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;
using System.IO;
using Serialization;
using CraftingGame.State;

namespace MainGame.UnitTests
{
    class GameHarness
    {
        private readonly Mock<IRenderer> mockRenderer;
        private readonly ITerrainGenerator terrain;
        private readonly InputMask playerInput;
        private readonly FrameCounter counter = new FrameCounter();


        public PlatformerSceene Game { get; private set; }

        internal void Update(float timeStep)
        {
            Game.Update(counter, timeStep);
        }

        internal void Render(float timeStep)
        {
            Game.Prerender(counter, timeStep);
            Game.Render(counter, timeStep);
        }

        public PlayerObject Player => Game.State.KnownPlayers.First();
        public Coordinate PlayerCoordinate => Game.Grid.PointToGridCoordinate(Player.Center);
        public Plane Plane => Player.Plane;

        // By default create a small 5x5 block viewport
        public static Vector2 DefaultViewPort => new Vector2(5 * PlatformerSceene.BlockSize, 5 * PlatformerSceene.BlockSize);

        internal static GameHarness CreateEmptyGame()
        {
            return CreateGameFromTerrain(new TerrainStub());
        }

        internal static GameHarness CreateSingleBlockGame(Coordinate blockCoord, TerrainType blockType)
        {
            var terrain = new TerrainStub();
            terrain.AddBlock(blockCoord.U, blockCoord.V, 0, new TerrainBlock() { Type = blockType });
            return CreateGameFromTerrain(terrain);
        }

        internal static GameHarness CreateSolidBlockGame(TerrainType blockType)
        {
            var terrain = new TerrainStub(c => c.V < 0 ? blockType : TerrainType.Free);
            return CreateGameFromTerrain(terrain);
        }

        internal static GameHarness CreateFromMap(string terrainMap)
        {
            return CreateGameFromTerrain(new TerrainStub(terrainMap));
        }

        private static GameHarness CreateGameFromTerrain(ITerrainGenerator terrain)
        {
            var mockRenderer = CreateRenderer(DefaultViewPort);
            return new GameHarness(mockRenderer, terrain);
        }

        private GameHarness(Mock<IRenderer> mockRenderer, ITerrainGenerator terrain)
        {
            this.mockRenderer = mockRenderer;
            this.terrain = terrain;
            this.playerInput = CreateInput("player1");
        }

        public void LoadGame(Stream stream)
        {
            this.Game = CreateGame(mockRenderer.Object, terrain);
            this.Game.LoadGame(stream);
            this.Game.Activate(CreateInput("ui"), GenerateInputSets(playerInput));
        }

        public void StartGame()
        {
            this.Game = CreateGame(mockRenderer.Object, terrain);
            this.Game.Activate(CreateInput("ui"), GenerateInputSets(playerInput));

            // Make player1 join the game
            this.Input();
            this.Update(0.1f);
        }

        public void Input(
            bool right = false,
            bool left = false,
            bool up = false,
            bool down = false,
            bool attack = false)
        {
            playerInput.Input.Active = true;
            playerInput.Input.Right = right;
            playerInput.Input.Left = left;
            playerInput.Input.Up = up;
            playerInput.Input.Down = down;
            playerInput.Input.Attack = attack;
        }

        public TerrainType ReadTerrainType(int u, int v)
        {
            return ReadTerrainType(new Coordinate(u, v));
        }

        public TerrainType ReadTerrainType(Coordinate c)
        {
            return Game.Terrain[c, Plane].Type;
        }

        public void VerifyTerrain(string terrainMap)
        {
            var generator = new TerrainGenerator(terrainMap);
            var offset = generator.Offset;
            var size = generator.Size;

            for(var v = 0; v <= size.V; v++)
            {
                for (var u = 0; u <= size.U; u++)
                {
                    var coord = new Coordinate(u - offset.U, v - offset.V);
                    var expectedTerrainType = generator.Generator(coord);
                    var actualTerrainType = ReadTerrainType(coord);
                    actualTerrainType.Should().Be(expectedTerrainType, $"terrain type at ({coord.U},{coord.V})");
                }
            }
        }

        public void VerifyBlockRendered(Coordinate coord)
        {
            VerifyBlockRendered(coord, Times.Once());
        }

        public void VerifyBlockRendered(Coordinate coord, Times times)
        {
            // We render blocks to a texture (not the screen)
            var blockSize = Game.Grid.Size;
            var blockLocation = Game.Grid.GridCoordinateToPoint(coord);
            var blockRenderLocation = new Vector2(0, 3000) + blockLocation.FlipY - Matrix2x2.ProjectY * Game.Grid.Size;

            mockRenderer.Verify(
                m => m.RenderRectangle(
                    It.Is<IReadonlyVector>(v => v.Equals(blockRenderLocation)),
                    It.Is<IReadonlyVector>(v => v.Equals(blockSize)),
                    It.IsAny<Color>()),
                times);
        }

        private static Mock<IRenderer> CreateRenderer(Vector2 viewPort)
        {
            Func<int, int, IGpuTexture> textureFactory = GpuTextureStub.Create;
            var mockScope = new Mock<IRenderScope>();
            mockScope.Setup(m => m.ResolveSprite(It.IsAny<IBinding<ISprite>>()))
                .Returns((IBinding<ISprite> b) => new ResolvedBinding<ISprite>(b, null));
            var mockRenderer = new Mock<IRenderer>();
            mockRenderer.Setup(m => m.GetDisplaySize()).Returns(viewPort);
            mockRenderer.Setup(m => m.InitializeGpuTexture(It.IsAny<int>(), It.IsAny<int>())).Returns(textureFactory);
            mockRenderer.Setup(m => m.ActivateScope(It.IsAny<string>())).Returns(mockScope.Object);
            mockRenderer.Setup(m => m.RenderToGpuTexture(It.IsAny<IGpuTexture>(), It.IsAny<Action>())).Callback((IGpuTexture t, Action a) => a());
            return mockRenderer;
        }

        private static PlatformerSceene CreateGame(IRenderer renderer, ITerrainGenerator generator)
        {
            var mockFactory = new Mock<ITerrainFactory>();
            mockFactory.Setup(m => m.Create()).Returns(generator);
            mockFactory.Setup(m => m.FromState(It.IsAny<StatefulObject>())).Returns(generator);
            return new PlatformerSceene(new InMemoryStreamProvider(), renderer, null, new ActionQueue(), mockFactory.Object);
        }

        private static InputMask[] GenerateInputSets(params InputMask[] players)
        {
            return new InputMask[4]
            {
                players.Length > 0 ? players[0] : CreateInput("player1"),
                players.Length > 1 ? players[1] : CreateInput("player2"),
                players.Length > 2 ? players[2] : CreateInput("player3"),
                players.Length > 3 ? players[3] : CreateInput("player4"),
            };
        }

        private static InputMask CreateInput(string playerBinding)
        {
            return new InputMask(playerBinding) { Input = new InputSet() { Active = false } };
        }
    }
}
