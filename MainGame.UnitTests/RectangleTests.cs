using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorMath;
using FluentAssertions;

namespace MainGame.UnitTests
{
    [TestClass]
    public class RectangleTests
    {
        [TestMethod]
        public void Intersection()
        {
            var rect = new Rect2(new Vector2(30, -30), new Vector2(30, 30));
            var rect2 = new Rect2(new Vector2(40, -40), new Vector2(10, 10));
            rect.Intersects(rect2).Should().BeTrue();
            rect2.Intersects(rect).Should().BeTrue();
        }
    }
}
