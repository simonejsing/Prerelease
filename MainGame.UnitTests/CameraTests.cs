using System;
using CraftingGame;
using CraftingGame.Physics;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorMath;

namespace MainGame.UnitTests
{
    [TestClass]
    public class CameraTests
    {
        [TestMethod]
        public void CameraCanFollowObject()
        {
            var viewPort = new Vector2(100, 100);
            var position = new Vector2(20, 70);
            var size = new Vector2(30, 30);
            var expected = 0.5f * viewPort.FlipX + position + 0.5f * size;

            var obj = new MovableObject(null, position, size);
            var view = new ViewportProjection(viewPort);
            var camera = new Camera(view);
            camera.Track(obj);
            camera.Follow();
            camera.Update();
            var topLeft = view.MapToWorld(Vector2.Zero);
            topLeft.Should().Be(expected);
        }
    }
}
