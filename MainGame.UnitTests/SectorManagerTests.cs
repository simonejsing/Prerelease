using System;
using CraftingGame;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainGame.UnitTests
{
    [TestClass]
    public class SectorManagerTests
    {
        [TestMethod]
        public void SectorManagerReturnsSectorConstruct()
        {
            var manager = new SectorManager(new TerrainStub());
            var sector = manager.GetSector(10, 20);
            sector.X.Should().Be(10);
            sector.Y.Should().Be(20);
        }
    }
}
