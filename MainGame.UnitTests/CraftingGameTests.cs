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
        public void CanInstantiateGame()
        {
            var mockRenderer = CreateRenderer();
            var game = CreateGame(mockRenderer.Object, new TerrainStub());
            game.Update(0.1f);
            game.Render(0.1f);
        }

        [TestMethod]
        public void RendersTerrainBlockWhenInsideView()
        {
            var mockRenderer = CreateRenderer();
            var terrain = new TerrainStub();
            terrain.AddBlock(0, 0, 0, new TerrainBlock() { Type = TerrainType.Rock });
            var game = CreateGame(mockRenderer.Object, terrain);
            game.ActiveView.TopLeft = new Vector2(-100, 100);
            game.Update(0.1f);
            game.Render(0.1f);
            mockRenderer.Verify(
                m => m.RenderRectangle(
                    It.Is<IReadonlyVector>(v => v.X == 100 && v.Y == 100),
                    It.Is<IReadonlyVector>(v => v.X == 30 && v.Y == 30),
                    It.IsAny<Color>()),
                Times.Once);
        }

        [TestMethod]
        public void HandlesPlayerMovingRight()
        {
            var playerInput = CreateInput();
            var game = CreateGame(CreateRenderer().Object, new TerrainStub(), playerInput);
            var player1 = game.State.Players.First();
            var initialX = player1.Position.X;
            ActivateInput(playerInput, moveRight: true);
            game.Update(0.1f);
            player1.Position.X.Should().BeGreaterThan(initialX);
        }

        private static void ActivateInput(
            InputMask playerInput,
            bool moveRight = false,
            bool moveLeft = false)
        {
            playerInput.Input.Active = true;
            playerInput.Input.Right = moveRight;
            playerInput.Input.Left = moveLeft;
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

        private static Mock<IRenderer> CreateRenderer()
        {
            var mockScope = new Mock<IRenderScope>();
            mockScope.Setup(m => m.ResolveSprite(It.IsAny<IBinding<ISprite>>()))
                .Returns((IBinding<ISprite>b) => new ResolvedBinding<ISprite>(b, null));
            var mockRenderer = new Mock<IRenderer>();
            mockRenderer.Setup(m => m.GetViewport()).Returns(new Vector2(800, -600));
            mockRenderer.Setup(m => m.ActivateScope(It.IsAny<string>())).Returns(mockScope.Object);
            return mockRenderer;
        }
    }
}
