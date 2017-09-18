using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class PhysicsEngine
    {
        private readonly ICollidableObjectGrid grid;
        private readonly List<ICollidableObject> collidableObjects = new List<ICollidableObject>();
        private readonly float timestep;

        public PhysicsEngine(ICollidableObjectGrid grid, float timestep)
        {
            this.grid = grid;
            this.timestep = timestep;
        }

        public void AddCollidableObject(ICollidableObject obj)
        {
            collidableObjects.Add(obj);
        }

        public void Apply(MovableObject obj, Vector2 instantVelocity)
        {
            // Apply gravity
            obj.Acceleration.Y += Constants.GRAVITY;

            // Accelerate object
            obj.Velocity += instantVelocity + obj.Acceleration * timestep;

            // Cap velocity
            if (Math.Abs(obj.Velocity.Y) > Constants.MAX_VERTICAL_VELOCITY)
            {
                obj.Velocity.Y = Math.Sign(obj.Velocity.Y) * Constants.MAX_VERTICAL_VELOCITY;
            }
            if (Math.Abs(obj.Velocity.X) > Constants.MAX_HORIZONTAL_VELOCITY)
            {
                obj.Velocity.X = Math.Sign(obj.Velocity.X) * Constants.MAX_HORIZONTAL_VELOCITY;
            }

            // Handle collisions with grid
            var deltaPosition = HandleGridCollisions(obj);

            // Handle collisions with collidable objects
            foreach (var collidableObject in collidableObjects)
            {
                HandleObjectCollision(obj, collidableObject, deltaPosition);
            }

            // Move object
            obj.Position += deltaPosition;
        }

        private static void HandleObjectCollision(MovableObject obj, ICollidableObject collidableObject, Vector2 deltaPosition)
        {
            if (collidableObject == obj)
                return;

            bool verticalCollision = false;
            bool horizontalCollision = false;

            var obj_x_min = obj.Position.X;
            var obj_x_max = obj_x_min + obj.Size.X;
            var obj_y_min = obj.Position.Y;
            var obj_y_max = obj_y_min + obj.Size.Y;
            var obj_center = obj.Center;
            var col_x_min = collidableObject.Position.X;
            var col_x_max = col_x_min + collidableObject.Size.X;
            var col_y_min = collidableObject.Position.Y;
            var col_y_max = col_y_min + collidableObject.Size.Y;

            // If vertical overlap, then check horizontal movement
            if (obj_y_max > col_y_min && obj_y_min < col_y_max)
            {
                // Test horizontal collision
                // On the left moving right
                if (obj_center.X < col_x_min && deltaPosition.X > 0)
                {
                    if (obj_x_max + deltaPosition.X > col_x_min)
                    {
                        horizontalCollision = true;
                        deltaPosition.X = col_x_min - obj_x_max;
                    }
                }
                // On the right moving left
                else if (obj_center.X > col_x_max && deltaPosition.X < 0)
                {
                    if (obj_x_min + deltaPosition.X < col_x_max)
                    {
                        horizontalCollision = true;
                        deltaPosition.X = col_x_max - obj_x_min;
                    }
                }

                // Can only collide with another object along one axis (because we are all boxes)
                //return;
            }

            // If horizontal overlap, then check vertical movement
            if (obj_x_max > col_x_min && obj_x_min < col_x_max)
            {
                // Test vertical collision
                // Above moving down
                if (obj_center.Y < col_y_min && deltaPosition.Y > 0)
                {
                    if (obj_y_max + deltaPosition.Y > col_y_min)
                    {
                        verticalCollision = true;
                        deltaPosition.Y = col_y_min - obj_y_max;
                    }
                }
                // Below moving up
                else if (obj_center.Y > col_y_max && deltaPosition.Y < 0)
                {
                    if (obj_y_min + deltaPosition.Y < col_y_max)
                    {
                        verticalCollision = true;
                        deltaPosition.Y = col_y_max - obj_y_min;
                    }
                }
            }

            // If a vertical collision is detected going downwards, the player has "landed" and he may accelerate
            if (verticalCollision && obj.Velocity.Y > 0)
                obj.CanAccelerate = true;

            // Kill velocity if a collision occured
            if (verticalCollision)
            {
                obj.Velocity.Y = 0;
            }
            if (horizontalCollision)
            {
                obj.Velocity.X = 0;
            }
        }

        private Vector2 HandleGridCollisions(MovableObject obj)
        {
            var neighboringBlocks = grid.Neighbors(obj);
            var centerBlock = neighboringBlocks[4];
            var deltaPosition = obj.Velocity * timestep;

            bool verticalCollision = false;
            bool horizontalCollision = false;

            // Vertical collision check
            // Check blocks 0-2 if going up
            // Check blocks 6-8 if going down
            if (deltaPosition.Y > 0)
            {
                // Going down
                if (neighboringBlocks[7].Occupied || neighboringBlocks[7 + Math.Sign(obj.Position.X - centerBlock.Position.X)].Occupied)
                {
                    // Check if collision emminent (the 4-index is intentional)
                    if (deltaPosition.Y > neighboringBlocks[4].Position.Y - obj.Position.Y)
                    {
                        // Cap vertical movement.
                        deltaPosition.Y = neighboringBlocks[4].Position.Y - obj.Position.Y;
                        verticalCollision = true;
                    }
                }
            }
            else if (deltaPosition.Y < 0)
            {
                // Going up
                if (neighboringBlocks[1].Occupied || neighboringBlocks[1 + Math.Sign(obj.Position.X - centerBlock.Position.X)].Occupied)
                {
                    // Check if collision emminent
                    if (-deltaPosition.Y + obj.Size.Y > obj.Position.Y - neighboringBlocks[1].Position.Y)
                    {
                        // Cap vertical movement.
                        deltaPosition.Y = -(obj.Position.Y - obj.Size.Y - neighboringBlocks[1].Position.Y);
                        verticalCollision = true;
                    }
                }
            }

            // Horizontal collision check
            // Check blocks 0,3,6 if going left
            // Check blocks 2,5,8 if going right
            // If vertical collision detected, then check only 3 and 5
            if (deltaPosition.X < 0)
            {
                // Going left
                if (neighboringBlocks[3].Occupied || (!verticalCollision && neighboringBlocks[3 + 3 * Math.Sign(obj.Position.Y - centerBlock.Position.Y)].Occupied))
                {
                    // Check if collision emminent (the 4-index is intentional)
                    if (-deltaPosition.X > obj.Position.X - neighboringBlocks[4].Position.X)
                    {
                        // Cap horizontal movement.
                        deltaPosition.X = -(obj.Position.X - neighboringBlocks[4].Position.X);
                        horizontalCollision = true;
                    }
                }
            }
            else if (deltaPosition.X > 0)
            {
                // Going right
                if (neighboringBlocks[5].Occupied || (!verticalCollision && neighboringBlocks[5 + 3 * Math.Sign(obj.Position.Y - centerBlock.Position.Y)].Occupied))
                {
                    // Check if collision emminent
                    if (deltaPosition.X + obj.Size.X > neighboringBlocks[5].Position.X - obj.Position.X)
                    {
                        // Cap vertical movement.
                        deltaPosition.X = neighboringBlocks[5].Position.X - obj.Position.X - obj.Size.X;
                        horizontalCollision = true;
                    }
                }
            }

            // If a vertical collision is detected going downwards, the player has "landed" and he may accelerate
            obj.CanAccelerate = verticalCollision && obj.Velocity.Y > 0;

            // Kill velocity if a collision occured
            if (verticalCollision)
            {
                obj.Velocity.Y = 0;
            }
            if (horizontalCollision)
            {
                obj.Velocity.X = 0;
            }

            return deltaPosition;
        }
    }
}
