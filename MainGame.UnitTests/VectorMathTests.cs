using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorMath;
using FluentAssertions;
using Terrain;

namespace MainGame.UnitTests
{
    [TestClass]
    public class VectorMathTests
    {
        [TestMethod]
        public void VectorEqualityComparisonEquals()
        {
            var v = new Vector2(12, 52);
            IReadonlyVector v2 = new Vector2(12, 52);

            (v == v2).Should().BeTrue();
        }

        [TestMethod]
        public void VectorEqualityComparisonNotEquals()
        {
            var v = new Vector2(12, 52);
            IReadonlyVector v2 = new Vector2(15, 52);

            (v == v2).Should().BeFalse();
        }

        [TestMethod]
        public void CanProjectVectorX()
        {
            var v = new Vector2(12, 52);
            var p = Matrix2x2.ProjectX * v;
            p.X.Should().Be(12);
            p.Y.Should().Be(0);
        }

        [TestMethod]
        public void CanProjectVectorY()
        {
            var v = new Vector2(12, 52);
            var p = Matrix2x2.ProjectY * v;
            p.X.Should().Be(0);
            p.Y.Should().Be(52);
        }

        [TestMethod]
        public void TestVectorAngleRight()
        {
            var v = new Vector2(1, 0);
            v.Angle.Should().Be(0);
        }

        [TestMethod]
        public void TestVectorAngleLeft()
        {
            var v = new Vector2(-1, 0);
            v.Angle.Should().Be((float)Math.PI);
        }

        [TestMethod]
        public void TestVectorAngleUp()
        {
            var v = new Vector2(0, 1);
            v.Angle.Should().Be((float)Math.PI/2.0f);
        }

        [TestMethod]
        public void TestVectorAngleDown()
        {
            var v = new Vector2(0, -1);
            v.Angle.Should().Be((float)-Math.PI/2.0f);
        }

        [TestMethod]
        public void TestManhattanDistance()
        {
            var c1 = new Coordinate(-1, -1);
            var c2 = new Coordinate(-1, -1);
            Coordinate.ManhattanDistance(c1, c2).Should().Be(0);
        }
    }
}
