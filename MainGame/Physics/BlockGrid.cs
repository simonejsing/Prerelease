using System;
using System.Collections.Generic;
using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public struct Block : ICollidableObject, IRenderableObject
    {
        private static readonly UnitVector2 blockFacing = UnitVector2.GetInstance(1, 0);

        public event ObjectCollisionEventHandler ObjectCollision;
        public event GridCollisionEventHandler GridCollision;
        public event HitEventHandler Hit;

        public void OnObjectCollision(ICollidableObject target, Collision collision)
        {
        }

        public void OnGridCollision(ICollidableObject[] target, Collision collision)
        {
        }

        public void OnHit(IProjectile target)
        {
        }

        public Rect2 BoundingBox => new Rect2(Position, Size);
        public Vector2 Position { get; set; }
        public UnitVector2 Facing => blockFacing;
        public Vector2 Size { get; set; }
        public IBinding<ISprite> SpriteBinding { get; set; }
        public Vector2 Center => Position + 0.5f*Size;
        public bool Occupied { get; set; }
    }

    public class BlockGrid : ICollidableObjectGrid
    {
        private readonly Block[,] grid;
        private readonly SharedObjectBinding<ISprite> sharedBinding;

        public Vector2 GridSize { get; }
        public uint GridWidth { get; }
        public uint GridHeight { get; }
        public uint Rows { get; }
        public uint Columns { get; }
        public IList<IRenderableObject> OccupiedBlocks { get; }

        public IBinding<ISprite> SpriteBinding
        {
            get { return sharedBinding.InnerBinding; }
            set { sharedBinding.InnerBinding = value; }
        }

        public BlockGrid(uint width, uint height, uint rows, uint columns)
        {
            sharedBinding = new SharedObjectBinding<ISprite>(new ObjectBinding<ISprite>("Block"));

            GridWidth = width;
            GridHeight = height;
            GridSize = new Vector2(GridWidth, GridHeight);
            Rows = rows;
            Columns = columns;

            grid = new Block[rows,columns];
            OccupiedBlocks = new List<IRenderableObject>();
        }

        public void Insert(uint row, uint column)
        {
            grid[row, column] = new Block()
            {
                Position = new Vector2(column * GridWidth, row * GridHeight),
                Size = GridSize,
                Occupied = true,
                SpriteBinding = sharedBinding
            };
            OccupiedBlocks.Add(grid[row, column]);
        }

        /// <summary>
        /// Returns an array representing a 3x3 grid of all neighbors around the given point.
        /// The input position can be outside of the grid or at the edge/corner and the result
        /// will still be 3x3 where tiles outside of the grid are unoccupied.
        /// </summary>
        public ICollidableObject[] Neighbors(Object obj)
        {
            var neighbors = new ICollidableObject[9];

            int gridX = (int) Math.Floor((obj.Position.X + obj.Size.X/2.0) / GridWidth);
            int gridY = (int) Math.Floor((obj.Position.Y + obj.Size.Y/2.0) / GridHeight);
            int localIndex = 0;

            for (int j = gridY - 1; j <= gridY + 1; j++)
            {
                for (int i = gridX - 1; i <= gridX + 1; i++)
                {
                    if (j < 0 || j >= Rows || i < 0 || i >= Columns)
                    {
                        neighbors[localIndex] = new Block() { Position = new Vector2(i * GridWidth, j * GridHeight), Occupied = false };
                    }
                    else
                    {
                        neighbors[localIndex] = new Block() { Position = new Vector2(i * GridWidth, j * GridHeight), Occupied = grid[j, i].Occupied };
                    }

                    localIndex++;
                }
            }

            return neighbors;
        }
    }
}