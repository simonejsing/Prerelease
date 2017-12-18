using Contracts;
using CraftingGame;
using CraftingGame.Physics;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;

namespace MainGame.UnitTests
{
    class GameHarness
    {
        private readonly Mock<IRenderer> mockRenderer;
        private readonly InputMask playerInput;

        public PlatformerSceene Game { get; }
        public PlayerObject Player => Game.State.Players.First();

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

        private static GameHarness CreateGameFromTerrain(ITerrainGenerator terrain)
        {
            var playerInput = CreateInput();
            var mockRenderer = CreateRenderer(DefaultViewPort);
            return new GameHarness(mockRenderer, terrain);
        }

        private GameHarness(Mock<IRenderer> mockRenderer, ITerrainGenerator terrain)
        {
            this.mockRenderer = mockRenderer;
            this.playerInput = CreateInput();
            this.Game = CreateGame(mockRenderer.Object, terrain, playerInput);
            this.Game.GenerateTerrainSectors();
        }

        public void Input(
            bool moveRight = false,
            bool moveLeft = false)
        {
            playerInput.Input.Active = true;
            playerInput.Input.Right = moveRight;
            playerInput.Input.Left = moveLeft;
        }

        public void VerifyBlockRendered(Coordinate coord)
        {
            VerifyBlockRendered(coord, Times.Once());
        }

        public void VerifyBlockRendered(Coordinate coord, Times times)
        {
            var blockSize = Game.Grid.Size;
            var view = Game.ActiveView;
            var blockLocation = Game.Grid.GridCoordinateToPoint(coord);
            var blockRenderLocation = blockLocation + Matrix2x2.ProjectY * blockSize - view.TopLeft;

            mockRenderer.Verify(
                m => m.RenderRectangle(
                    It.Is<IReadonlyVector>(v => v.Equals(blockRenderLocation)),
                    It.Is<IReadonlyVector>(v => v.Equals(blockSize.FlipY)),
                    It.IsAny<Color>()),
                times);
        }

        private static Mock<IRenderer> CreateRenderer(Vector2 viewPort)
        {
            var mockScope = new Mock<IRenderScope>();
            mockScope.Setup(m => m.ResolveSprite(It.IsAny<IBinding<ISprite>>()))
                .Returns((IBinding<ISprite> b) => new ResolvedBinding<ISprite>(b, null));
            var mockRenderer = new Mock<IRenderer>();
            mockRenderer.Setup(m => m.GetViewport()).Returns(viewPort);
            mockRenderer.Setup(m => m.ActivateScope(It.IsAny<string>())).Returns(mockScope.Object);
            return mockRenderer;
        }

        private static PlatformerSceene CreateGame(IRenderer renderer, ITerrainGenerator generator, params InputMask[] players)
        {
            var game = new PlatformerSceene(renderer, null, new ActionQueue(), generator);
            game.Activate(CreateInput(), GenerateInputSets(players));
            return game;
        }

        private static InputMask[] GenerateInputSets(params InputMask[] players)
        {
            return new InputMask[4]
            {
                players.Length > 0 ? players[0] : CreateInput(),
                players.Length > 1 ? players[1] : CreateInput(),
                players.Length > 2 ? players[2] : CreateInput(),
                players.Length > 3 ? players[3] : CreateInput(),
            };
        }

        private static InputMask CreateInput()
        {
            return new InputMask { Input = new InputSet() { Active = false } };
        }
    }
}
