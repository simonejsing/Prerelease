using System;
using System.Linq;
using CraftingGame;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Terrain;

namespace MainGame.UnitTests
{
    [TestClass]
    public class TerrainSectorTests
    {
        private const int GridU = 10;
        private const int GridV = 20;
        private const int Plane = 5;

        [TestMethod]
        public void SectorIsInitializedWithNotGeneratedBlocks()
        {
            const int x = 10;
            const int y = 15;
            var sector = new TerrainSector(new TerrainStub(), GridU, GridV, Plane);
            sector.U.Should().Be(GridU);
            sector.V.Should().Be(GridV);
            sector.W.Should().Be(Plane);
            var tile = sector[x, y];
            tile.X.Should().Be(TerrainSector.SectorWidth * GridU + x);
            tile.Y.Should().Be(TerrainSector.SectorHeight * GridV + y);
            tile.Type.Should().Be(TerrainType.NotGenerated);
        }

        [TestMethod]
        public void SectorLoadsTilesDuringUpdate()
        {
            var terrainStub = new TerrainStub();
            terrainStub.AddBlock(
                GridU * TerrainSector.SectorWidth,
                GridV * TerrainSector.SectorHeight,
                Plane,
                new TerrainBlock(){Type = TerrainType.Dirt});
            var sector = new TerrainSector(terrainStub, GridU, GridV, Plane);
            sector.Update(1).Should().Be(1);
            sector[0, 0].Type.Should().Be(TerrainType.Dirt);
        }

        [TestMethod]
        public void CanForceGenerateTile()
        {
            const int x = 33;
            const int y = 55;
            var terrainStub = new TerrainStub();
            terrainStub.AddBlock(
                GridU * TerrainSector.SectorWidth + x,
                GridV * TerrainSector.SectorHeight + y,
                Plane,
                new TerrainBlock() { Type = TerrainType.Dirt });
            var sector = new TerrainSector(terrainStub, GridU, GridV, Plane);
            sector.Generate(x, y);
            var tile = sector[x, y];
            tile.Type.Should().Be(TerrainType.Dirt);
        }

        [TestMethod]
        public void SectorIsMarkedCompleteWhenFullyLoaded()
        {
            var terrainStub = new TerrainStub();
            var sector = new TerrainSector(terrainStub, GridU, GridV, Plane);
            sector.Update(-1).Should().Be(TerrainSector.SectorWidth * TerrainSector.SectorHeight);
            sector[TerrainSector.SectorWidth - 1, TerrainSector.SectorHeight - 1].Type.Should().Be(TerrainType.Free);
            sector.FullyLoaded.Should().BeTrue();
        }

        [TestMethod]
        public void CachedTerrainQueriesGeneratorOnce()
        {
            var terrainStub = new TerrainStub();
            terrainStub.AddBlock(125, 135, 12, new TerrainBlock() { Type = TerrainType.Dirt });
            var generator = new CachedTerrainGenerator(terrainStub);
            generator.Generate(125, 135, 12);
            var block = generator[125, 135, 12];
            var block2 = generator[125, 135, 12];
            block.Type.Should().Be(TerrainType.Dirt);
            block2.Type.Should().Be(TerrainType.Dirt);
            terrainStub.GenerationCounter().Should().Be(1);
        }

        [TestMethod]
        public void CachedTerrainSeamlesslyCreatesNewSectors()
        {
            var generator = new CachedTerrainGenerator(new TerrainStub());
            generator.Generate(125, 135, 12);
            generator.Generate(125 + TerrainSector.SectorWidth, 135, 12);
            generator.Generate(125, 135 + TerrainSector.SectorHeight, 12);
            generator.Generate(125, 135, 12 + 1);
            generator.Sectors.Should().HaveCount(4);
            generator[125, 135, 12].Type.Should().Be(TerrainType.Free);
            generator[125 + TerrainSector.SectorWidth, 135, 12].Type.Should().Be(TerrainType.Free);
            generator[125, 135 + TerrainSector.SectorHeight, 12].Type.Should().Be(TerrainType.Free);
            generator[125, 135, 12 + 1].Type.Should().Be(TerrainType.Free);
        }

        [TestMethod]
        public void CachedTerrainWorksWithNegativeCoordinates()
        {
            var generator = new CachedTerrainGenerator(new TerrainStub());
            const int negativeX = -10 - TerrainSector.SectorWidth;
            const int negativeY = -15 - TerrainSector.SectorHeight;
            generator.Generate(negativeX, negativeY, 0);
            generator.Sectors.Should().HaveCount(1);
            generator[negativeX, negativeY, 0].Type.Should().Be(TerrainType.Free);
        }
    }
}
