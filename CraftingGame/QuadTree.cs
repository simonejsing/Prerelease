using System;
using System.Collections.Generic;
using System.Text;
using Terrain;

namespace CraftingGame
{
    public class QuadNode<TNode> where TNode : class
    {
        public Voxel Voxel { get; }
        public TNode Value { get; }
        public QuadNode<TNode> Left { get; set; }
        public QuadNode<TNode> Right { get; set; }
        public QuadNode<TNode> Up { get; set; }
        public QuadNode<TNode> Down { get; set; }

        internal QuadNode(Voxel coord, TNode value)
        {
            Voxel = coord;
            Value = value;
        }
    }

    public class QuadTree<TTree> where TTree : class
    {
        private Dictionary<Voxel, QuadNode<TTree>> cache = new Dictionary<Voxel, QuadNode<TTree>>();
        private QuadNode<TTree> pointer;

        public QuadTree()
        {
            pointer = null;
        }

        public void Add(Voxel v, TTree value)
        {
            var node = new QuadNode<TTree>(v, value);

            // Update neighbor relations
            var toLeft = FindNode(new Voxel(v.Coordinate + new Coordinate(-1, 0), v.Plane));
            var toRight = FindNode(new Voxel(v.Coordinate + new Coordinate(1, 0), v.Plane));
            var above = FindNode(new Voxel(v.Coordinate + new Coordinate(0, 1), v.Plane));
            var below = FindNode(new Voxel(v.Coordinate + new Coordinate(0, -1), v.Plane));
            node.Left = toLeft;
            if (toLeft != null)
                toLeft.Right = node;
            node.Right = toRight;
            if (toRight != null)
                toRight.Left = node;
            node.Up = above;
            if (above != null)
                above.Down = node;
            node.Down = below;
            if (below != null)
                below.Up = node;

            cache.Add(v, node);
        }

        public TTree this[Voxel c]
        {
            get
            {
                var node = FindNode(c);
                return node?.Value;
            }
        }

        private QuadNode<TTree> FindNode(Voxel v)
        {
            if (pointer == null)
            {
                return TryLookupCache(v);
            }

            // If different plane then we can't seek to it
            if (pointer.Voxel.W != v.W)
            {
                return TryLookupCache(v);
            }

            var distance = Coordinate.ManhattanDistance(pointer.Voxel.Coordinate, v.Coordinate);
            if (distance > 10)
            {
                return TryLookupCache(v);
            }

            return Seek(v);
        }

        private QuadNode<TTree> Seek(Voxel v)
        {
            var distance = Coordinate.ManhattanDistance(pointer.Voxel.Coordinate, v.Coordinate);
            while (v != pointer.Voxel)
            {
                // Seek U
                if (v.U > pointer.Voxel.U)
                {
                    pointer = pointer.Right;
                }
                else if (v.U < pointer.Voxel.U)
                {
                    pointer = pointer.Left;
                }

                // Guard against gaps in quad tree
                if(pointer == null)
                {
                    return TryLookupCache(v);
                }

                // Seek V
                if (v.V > pointer.Voxel.V)
                {
                    pointer = pointer.Up;
                }
                else if (v.V < pointer.Voxel.V)
                {
                    pointer = pointer.Down;
                }

                // Guard against gaps in quad tree
                if (pointer == null)
                {
                    return TryLookupCache(v);
                }

                // Distance should decrease otherwise there's a bug
                var newDistance = Coordinate.ManhattanDistance(pointer.Voxel.Coordinate, v.Coordinate);
                if(newDistance >= distance)
                {
                    throw new Exception("Error seeking quad tree, distance did not decrease during iteration.");
                }
            }

            return pointer;
        }

        private QuadNode<TTree> TryLookupCache(Voxel v)
        {
            if (cache.TryGetValue(v, out pointer))
            {
                return pointer;
            }

            return null;
        }
    }
}
