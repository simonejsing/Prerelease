﻿using Contracts;
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

namespace MainGame.UnitTests
{
    class GameHarness
    {
        private readonly Mock<IRenderer> mockRenderer;
        private readonly InputMask playerInput;

        public PlatformerSceene Game { get; }
        public PlayerObject Player => Game.State.Players.First();
        public Coordinate PlayerCoordinate => Game.Grid.PointToGridCoordinate(Player.Center);
        public Plane Plane => Game.Plane;

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
            this.playerInput = CreateInput();
            this.Game = CreateGame(mockRenderer.Object, terrain, playerInput);
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
            var blockSize = Game.View.MapSizeToViewport(Game.Grid.Size);
            var blockLocation = Game.Grid.GridCoordinateToPoint(coord);
            var blockRenderLocation = Game.View.MapToViewport(blockLocation + Matrix2x2.ProjectY*Game.Grid.Size);

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
