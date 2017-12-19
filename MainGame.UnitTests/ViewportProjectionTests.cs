using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CraftingGame;
using VectorMath;
using FluentAssertions;

namespace MainGame.UnitTests
{
    [TestClass]
    public class ViewportProjectionTests
    {
        [TestMethod]
        public void DefaultViewportProjection()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            VerifyProjection(
                projection,
                new Vector2(0, 0),
                new Vector2(0, 0));
            VerifyProjection(
                projection,
                new Vector2(10, 10),
                new Vector2(10, 10));
        }

        [TestMethod]
        public void ViewportProjectionTranslation()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            projection.Translate(new Vector2(-50, 20));
            VerifyProjection(
                projection,
                new Vector2(0, 0),
                new Vector2(50, -20));
        }

        [TestMethod]
        public void ViewportProjectionScale()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            projection.Scale(new Vector2(1.1f, 2.0f));
            VerifyProjection(
                projection,
                new Vector2(11, 20),
                new Vector2(10, 10));
        }

        [TestMethod]
        public void ViewportProjectionCenter()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            projection.Center(new Vector2(0, 0));
            VerifyProjection(
                projection,
                new Vector2(-50, 50),
                new Vector2(0, 0));
        }

        [TestMethod]
        public void ViewportProjectionCenterThenScale()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            projection.Center(new Vector2(0, 0));
            projection.Scale(2.0f);
            VerifyProjection(
                projection,
                new Vector2(-100, 100),
                new Vector2(0, 0));
        }

        [TestMethod]
        public void ViewportProjectionToWorld()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            projection.Center(new Vector2(0, 0));
            var p = projection.Projection;
            p.TopLeft.X.Should().Be(-50);
            p.TopLeft.Y.Should().Be(50);
            p.BottomRight.X.Should().Be(50);
            p.BottomRight.Y.Should().Be(-50);
        }

        [TestMethod]
        public void ViewportProjectionToWorldScaled()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            projection.Scale(2.0f);
            var p = projection.Projection;
            p.TopLeft.X.Should().Be(0);
            p.TopLeft.Y.Should().Be(0);
            p.BottomRight.X.Should().Be(200);
            p.BottomRight.Y.Should().Be(-200);
        }

        [TestMethod]
        public void ViewportProjectionToWorldScaledAroundCenter()
        {
            var projection = new ViewportProjection(new Vector2(100, 100));
            projection.Center(new Vector2(0, 0));
            projection.Scale(2.0f);
            var p = projection.Projection;
            p.TopLeft.X.Should().Be(-100);
            p.TopLeft.Y.Should().Be(100);
            p.BottomRight.X.Should().Be(100);
            p.BottomRight.Y.Should().Be(-100);
        }

        private static void VerifyProjection(ViewportProjection projection, Vector2 point, Vector2 projectedPoint)
        {
            // Test projection from world to viewport
            var p = projection.MapToViewport(point);
            p.X.Should().Be(projectedPoint.X, "X coordinate should match");
            p.Y.Should().Be(projectedPoint.Y, "Y coordinate should match");

            // Test reverse projection from viewport to world
            var wp = projection.MapToWorld(projectedPoint);
            wp.X.Should().Be(point.X, "X coordinate should match");
            wp.Y.Should().Be(point.Y, "Y coordinate should match");
        }
    }
}
