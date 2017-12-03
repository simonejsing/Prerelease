using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;

namespace MainGame.UnitTests
{
    [TestClass]
    public class TerrainGeneratorTests
    {
        [TestMethod]
        public void GeneratorReturnsFreeWhenBelowMaxDepth()
        {
            const int maxDepth = 100;
            var generator = new Generator(maxDepth);
            generator[0, -maxDepth -1, 0].Type.Should().Be(TerrainType.Free);
        }
    }
}
