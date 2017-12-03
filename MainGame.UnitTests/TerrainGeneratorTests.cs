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

        [TestMethod]
        public void TestPerlinNoise()
        {
            var noise = new PerlinNoise(0);
            var value = noise.perlin(2, 1);
            value.Should().Be(0);
        }
    }
}
