using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorMath;
using FluentAssertions;

namespace MainGame.UnitTests
{
    [TestClass]
    public class MatrixTests
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
    }
}
