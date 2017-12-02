using System;
using Contracts;
using CraftingGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VectorMath;

namespace MainGame.UnitTests
{
    [TestClass]
    public class CraftingGameTests
    {
        [TestMethod]
        public void CanInstantiateGame()
        {
            var playerInput = GenerateInput();
            var mockRenderer = CreateRenderer();
            var game = CreateGame(mockRenderer.Object, playerInput);
            game.Update(0.1f);
            game.Render(0.1f);
        }

        private static PlatformerSceene CreateGame(IRenderer renderer, params InputSet[] players)
        {
            var game = new PlatformerSceene(renderer, null, new ActionQueue());
            game.Activate(GenerateInputSets(players));
            return game;
        }

        private static InputSet GenerateInput()
        {
            return new InputSet() { Active = true };
        }

        private static InputMask[] GenerateInputSets(params InputSet[] players)
        {
            return new InputMask[4]
            {
                new InputMask() { Input = players.Length > 0 ? players[0] : DefaultInputSet() },
                new InputMask() { Input = players.Length > 1 ? players[1] : DefaultInputSet() },
                new InputMask() { Input = players.Length > 2 ? players[2] : DefaultInputSet() },
                new InputMask() { Input = players.Length > 3 ? players[3] : DefaultInputSet() },
            };
        }

        private static InputSet DefaultInputSet()
        {
            return new InputSet() { Active = false };
        }

        private static Mock<IRenderer> CreateRenderer()
        {
            var mockScope = new Mock<IRenderScope>();
            mockScope.Setup(m => m.ResolveSprite(It.IsAny<IBinding<ISprite>>()))
                .Returns((IBinding<ISprite>b) => new ResolvedBinding<ISprite>(b, new StubSprite()));
            var mockRenderer = new Mock<IRenderer>();
            mockRenderer.Setup(m => m.GetViewport()).Returns(new Vector2(800, 600));
            mockRenderer.Setup(m => m.ActivateScope(It.IsAny<string>())).Returns(mockScope.Object);
            return mockRenderer;
        }
    }

    internal class StubSprite : ISprite
    {
        public IReadonlyRectangle SourceRectangle { get; }
        public Vector2 Size { get; set; }

        public StubSprite()
        {
            SourceRectangle = null;
            Size = new Vector2(30, 30);
        }
    }
}
