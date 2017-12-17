using System;
using CraftingGame;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terrain;
using VectorMath;

namespace MainGame.UnitTests
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void CoordinateAddition()
        {
            var c1 = new Coordinate(1, 2);
            var c2 = new Coordinate(1, 2);
            (c1 + c2).U.Should().Be(2);
            (c1 + c2).V.Should().Be(4);
        }

        [TestMethod]
        public void CoordinateSubtraction()
        {
            var c1 = new Coordinate(1, 7);
            var c2 = new Coordinate(2, 2);
            (c1 - c2).U.Should().Be(-1);
            (c1 - c2).V.Should().Be(5);
        }

        [TestMethod]
        public void CoordinateMultiplication()
        {
            var c1 = new Coordinate(1, 7);
            var c2 = new Coordinate(2, 2);
            (c1 * c2).U.Should().Be(2);
            (c1 * c2).V.Should().Be(14);
        }

        [TestMethod]
        public void CoordinateDivision()
        {
            var c1 = new Coordinate(4, 9);
            var c2 = new Coordinate(2, 2);
            (c1 / c2).U.Should().Be(2);
            (c1 / c2).V.Should().Be(4);
        }

        [TestMethod]
        public void GridPointInFirstQuadrant()
        {
            var grid = new Grid(new Vector2(30, 30));
            var coord = grid.PointToGridCoordinate(40, 80);
            coord.U.Should().Be(1);
            coord.V.Should().Be(2);
            var p = grid.GridCoordinateToPoint(coord);
            p.X.Should().Be(30);
            p.Y.Should().Be(60);
        }

        [TestMethod]
        public void GridPointInSecondQuadrant()
        {
            var grid = new Grid(new Vector2(30, 30));
            var coord = grid.PointToGridCoordinate(40, -80);
            coord.U.Should().Be(1);
            coord.V.Should().Be(-3);
            var p = grid.GridCoordinateToPoint(coord);
            p.X.Should().Be(30);
            p.Y.Should().Be(-90);
        }

        [TestMethod]
        public void GridPointInThirdQuadrant()
        {
            var grid = new Grid(new Vector2(30, 30));
            var coord = grid.PointToGridCoordinate(-40, 80);
            coord.U.Should().Be(-2);
            coord.V.Should().Be(2);
            var p = grid.GridCoordinateToPoint(coord);
            p.X.Should().Be(-60);
            p.Y.Should().Be(60);
        }

        [TestMethod]
        public void GridPointInFourthQuadrant()
        {
            var grid = new Grid(new Vector2(30, 30));
            var coord = grid.PointToGridCoordinate(-40, -80);
            coord.U.Should().Be(-2);
            coord.V.Should().Be(-3);
            var p = grid.GridCoordinateToPoint(coord);
            p.X.Should().Be(-60);
            p.Y.Should().Be(-90);
        }
    }
}
