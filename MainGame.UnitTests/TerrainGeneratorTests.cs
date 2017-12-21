using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using Contracts;

namespace MainGame.UnitTests
{
    [TestClass]
    public class TerrainGeneratorTests
    {
        private static readonly Plane plane = new Plane(0);

        [TestMethod]
        public void GeneratorReturnsFreeWhenBelowMaxDepth()
        {
            const int maxDepth = 100;
            const int maxHeight = 200;
            var generator = new Generator(maxDepth, maxHeight, 0);
            var coord = new Coordinate(0, -maxDepth - 1);
            generator[coord, plane].Type.Should().Be(TerrainType.Free);
        }

        [TestMethod]
        public void GeneratorReturnsFreeWhenAboveMaxHeight()
        {
            const int maxDepth = 200;
            const int maxHeight = 100;
            var generator = new Generator(maxDepth, maxHeight, 0);
            var coord = new Coordinate(0, maxHeight + 1);
            generator[coord, plane].Type.Should().Be(TerrainType.Free);
        }

        [TestMethod]
        public void TestPerlinNoise()
        {
            var noise = new PerlinNoise(0);
            var value = noise.Noise(2, 1, 1.0, 1.0, 0.0, 1.0);
            value.Should().Be(0.5);
        }
    }
}
