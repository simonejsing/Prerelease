using System;
using System.Linq;
using CraftingGame;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;

namespace MainGame.UnitTests
{
    [TestClass]
    public class TerrainSectorTests
    {
        const int GridX = 10;
        const int GridY = 20;

        [TestMethod]
        public void SectorIsInitializedWithNotGeneratedBlocks()
        {
            var sector = new TerrainSector(new TerrainStub().Plane(0), GridX, GridY);
            sector.X.Should().Be(GridX);
            sector.Y.Should().Be(GridY);
            var firstTile = sector.Tiles[0, 0];
            firstTile.X.Should().Be(TerrainSector.SectorWidth * GridX);
            firstTile.Y.Should().Be(TerrainSector.SectorHeight * GridY);
            firstTile.Type.Should().Be(TerrainType.NotGenerated);
        }

        [TestMethod]
        public void SectorLoadsTilesDuringUpdate()
        {
            var terrainStub = new TerrainStub();
            terrainStub.AddBlock(GridX * TerrainSector.SectorWidth, GridY * TerrainSector.SectorHeight, 0, new TerrainBlock(){Type = TerrainType.Dirt});
            var sector = new TerrainSector(terrainStub.Plane(0), GridX, GridY);
            sector.Update(1);
            sector.Tiles[0, 0].Type.Should().Be(TerrainType.Dirt);
        }

        [TestMethod]
        public void CanForceGenerateTile()
        {
            const int sectorX = 33;
            const int sectorY = 55;
            var terrainStub = new TerrainStub();
            terrainStub.AddBlock(GridX * TerrainSector.SectorWidth + sectorX, GridY * TerrainSector.SectorHeight + sectorY, 0, new TerrainBlock() { Type = TerrainType.Dirt });
            var sector = new TerrainSector(terrainStub.Plane(0), GridX, GridY);
            sector.Generate(sectorX, sectorY);
            var tile = sector.Tiles[sectorX, sectorY];
            tile.Type.Should().Be(TerrainType.Dirt);
        }

        [TestMethod]
        public void SectorIsMarkedCompleteWhenFullyLoaded()
        {
            var terrainStub = new TerrainStub();
            var sector = new TerrainSector(terrainStub.Plane(0), GridX, GridY);
            sector.Update(TerrainSector.SectorWidth * TerrainSector.SectorHeight);
            sector.Tiles[TerrainSector.SectorWidth - 1, TerrainSector.SectorHeight - 1].Type.Should().Be(TerrainType.Free);
            sector.FullyLoaded.Should().BeTrue();
        }
    }
}
