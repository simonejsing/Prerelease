using System;
using System.Linq;
using CraftingGame;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Terrain;
using Contracts;

namespace MainGame.UnitTests
{
    [TestClass]
    public class TerrainSectorTests
    {
        private const int GridU = 10;
        private const int GridV = 20;
        private const int DefaultPlane = 5;

        [TestMethod]
        public void SectorIsInitializedWithNotGeneratedBlocks()
        {
            const int x = 10;
            const int y = 15;
            var sector = new TerrainSector(new TerrainStub(), GridU, GridV, DefaultPlane);
            sector.U.Should().Be(GridU);
            sector.V.Should().Be(GridV);
            sector.W.Should().Be(DefaultPlane);
            var tile = sector[x, y];
            tile.Coord.U.Should().Be(TerrainSector.SectorWidth * GridU + x);
            tile.Coord.V.Should().Be(TerrainSector.SectorHeight * GridV + y);
            tile.Type.Should().Be(TerrainType.NotGenerated);
        }

        [TestMethod]
        public void SectorLoadsTilesDuringUpdate()
        {
            var terrainStub = new TerrainStub();
            terrainStub.AddBlock(
                new Coordinate(
                    GridU * TerrainSector.SectorWidth,
                    GridV * TerrainSector.SectorHeight),
                new Plane(DefaultPlane),
                new TerrainBlock(){Type = TerrainType.Dirt});
            var sector = new TerrainSector(terrainStub, GridU, GridV, DefaultPlane);
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
                new Coordinate(
                    GridU * TerrainSector.SectorWidth + x,
                    GridV * TerrainSector.SectorHeight + y),
                new Plane(DefaultPlane),
                new TerrainBlock() { Type = TerrainType.Dirt });
            var sector = new TerrainSector(terrainStub, GridU, GridV, DefaultPlane);
            sector.Generate(x, y);
            var tile = sector[x, y];
            tile.Type.Should().Be(TerrainType.Dirt);
        }

        [TestMethod]
        public void SectorIsMarkedCompleteWhenFullyLoaded()
        {
            var terrainStub = new TerrainStub();
            var sector = new TerrainSector(terrainStub, GridU, GridV, DefaultPlane);
            sector.Update(-1).Should().Be(TerrainSector.SectorWidth * TerrainSector.SectorHeight);
            sector[TerrainSector.SectorWidth - 1, TerrainSector.SectorHeight - 1].Type.Should().Be(TerrainType.Free);
            sector.FullyLoaded.Should().BeTrue();
        }

        [TestMethod]
        public void CachedTerrainQueriesGeneratorOnce()
        {
            var coord = new Coordinate(125, 135);
            var plane = new Plane(12);
            var terrainStub = new TerrainStub();
            terrainStub.AddBlock(
                coord,
                plane,
                new TerrainBlock() { Type = TerrainType.Dirt });
            var generator = new CachedTerrainGenerator(terrainStub);
            generator.Generate(coord, plane);
            var block = generator[coord, plane];
            var block2 = generator[coord, plane];
            block.Type.Should().Be(TerrainType.Dirt);
            block2.Type.Should().Be(TerrainType.Dirt);
            terrainStub.GenerationCounter().Should().Be(1);
        }

        [TestMethod]
        public void CachedTerrainSeamlesslyCreatesNewSectors()
        {
            var coord = new Coordinate(125, 135);
            var width = new Coordinate(TerrainSector.SectorWidth, 0);
            var height = new Coordinate(0, TerrainSector.SectorHeight);
            var plane = new Plane(12);
            var generator = new CachedTerrainGenerator(new TerrainStub());
            generator.Generate(coord, plane);
            generator.Generate(coord + width, plane);
            generator.Generate(coord + height, plane);
            generator.Generate(coord, new Plane(12 + 1));
            generator.Sectors.Should().HaveCount(4);
            generator[coord, plane].Type.Should().Be(TerrainType.Free);
            generator[coord + width, plane].Type.Should().Be(TerrainType.Free);
            generator[coord + height, plane].Type.Should().Be(TerrainType.Free);
            generator[coord, new Plane(12 + 1)].Type.Should().Be(TerrainType.Free);
        }

        [TestMethod]
        public void CachedTerrainWorksWithNegativeCoordinates()
        {
            var generator = new CachedTerrainGenerator(new TerrainStub());
            var coord = new Coordinate(-10 - TerrainSector.SectorWidth, -15 - TerrainSector.SectorHeight);
            generator.Generate(coord, new Plane(0));
            generator.Sectors.Should().HaveCount(1);
            generator[coord, new Plane(0)].Type.Should().Be(TerrainType.Free);
        }

        [TestMethod]
        public void CachedTerrainWorksWithNegativeCoordinates_OffByOne()
        {
            var generator = new CachedTerrainGenerator(new TerrainStub());
            var coord = new Coordinate(-10 - TerrainSector.SectorWidth, -100);
            generator.Generate(coord, new Plane(0));
            generator.Sectors.Should().HaveCount(1);
            generator[coord, new Plane(0)].Type.Should().Be(TerrainType.Free);
        }

        [TestMethod]
        public void ShouldFireEventOnModificationOfTerrain()
        {
            var sector = new TerrainSector(new TerrainStub(), 10, 15, 2);
            EventTracer.With<TerrainModificationEvent>(handler => sector.TerrainModification += handler)
                .Action(() => sector.Modify(5, 5, TerrainType.Dirt))
                .RaisedEvent(
                    e => 
                        e.Index == new Voxel(10, 15, 2) && 
                        e.LocalCoord == new Coordinate(5, 5) &&
                        e.GlobalCoord == new Coordinate(10 * TerrainSector.SectorWidth, 15 * TerrainSector.SectorHeight) + new Coordinate(5, 5))
                .Should().BeTrue();
        }
    }
}
