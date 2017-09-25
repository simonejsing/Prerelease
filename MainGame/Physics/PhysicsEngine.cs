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
        private readonly IEnumerable<MovableObject> movableObjects;
        private readonly float timestep;

        public PhysicsEngine(ICollidableObjectGrid grid, IEnumerable<MovableObject> movableObjects, float timestep)
        {
            this.grid = grid;
            this.movableObjects = movableObjects;
            this.timestep = timestep;
        }

        public void ApplyToObject(MovableObject obj, Vector2 instantVelocity)
        {
            obj.DeltaPosition.Clear();

            // Apply gravity
            obj.Acceleration.Y += Constants.GRAVITY;

            // Accelerate object
            obj.Velocity += instantVelocity + obj.Acceleration * timestep;

            // Slow down object when on ground
            if (obj.Grounded)
            {
                obj.Velocity.X *= 0.9f;
            }

            // Cap velocity
            if (Math.Abs(obj.Velocity.Y) > Constants.MAX_VERTICAL_VELOCITY)
            {
                obj.Velocity.Y = Math.Sign(obj.Velocity.Y) * Constants.MAX_VERTICAL_VELOCITY;
            }
            if (Math.Abs(obj.Velocity.X) > Constants.MAX_HORIZONTAL_VELOCITY)
            {
                obj.Velocity.X = Math.Sign(obj.Velocity.X) * Constants.MAX_HORIZONTAL_VELOCITY;
            }

            obj.DeltaPosition = obj.Velocity * timestep;

            // Handle collisions with grid
            obj.Grounded = false;
            HandleGridCollisions(obj);

            // Handle collisions with collidable objects
            foreach (var collidableObject in movableObjects)
            {
                HandleObjectCollision(obj, collidableObject);
            }

            // Move object
            obj.Position += obj.DeltaPosition;
        }

        private static void HandleObjectCollision(MovableObject obj, MovableObject movableObject)
        {
            if (movableObject == obj)
                return;

            bool verticalCollision = false;
            bool horizontalCollision = false;

            var obj_min = obj.Position;
            var obj_max = obj_min + obj.Size;
            var obj_center = obj.Center;
            var col_min = movableObject.Position;
            var col_max = col_min + movableObject.Size;

            // If vertical overlap, then check horizontal movement
            if (obj_max.Y > col_min.Y && obj_min.Y < col_max.Y)
            {
                // Test horizontal collision
                // On the left moving right
                if (obj_center.X < col_min.X && obj.DeltaPosition.X > 0)
                {
                    if (obj_max.X + obj.DeltaPosition.X > col_min.X)
                    {
                        horizontalCollision = true;
                        obj.DeltaPosition.X = col_min.X - obj_max.X;
                    }
                }
                // On the right moving left
                else if (obj_center.X > col_max.X && obj.DeltaPosition.X < 0)
                {
                    if (obj_min.X + obj.DeltaPosition.X < col_max.X)
                    {
                        horizontalCollision = true;
                        obj.DeltaPosition.X = col_max.X - obj_min.X;
                    }
                }
            }

            // If horizontal overlap, then check vertical movement
            if (obj_max.X > col_min.X && obj_min.X < col_max.X)
            {
                // Test vertical collision
                // Above moving down
                if (obj_center.Y < col_min.Y && obj.DeltaPosition.Y > 0)
                {
                    if (obj_max.Y + obj.DeltaPosition.Y > col_min.Y)
                    {
                        verticalCollision = true;
                        obj.DeltaPosition.Y = col_min.Y - obj_max.Y;
                    }
                }
                // Below moving up
                else if (obj_center.Y > col_max.Y && obj.DeltaPosition.Y < 0)
                {
                    if (obj_min.Y + obj.DeltaPosition.Y < col_max.Y)
                    {
                        verticalCollision = true;
                        obj.DeltaPosition.Y = col_max.Y - obj_min.Y;
                    }
                }
            }

            if (verticalCollision || horizontalCollision)
            {
                var collision = new Collision()
                {
                    Force = obj.DeltaPosition,
                    HorizontalCollision = horizontalCollision,
                    VerticalCollision = verticalCollision
                };
                movableObject.OnCollision(obj, collision);
                obj.OnCollision(movableObject, collision);
            }

            // Transfer momentum to movable object during collision.
            if (horizontalCollision)
            {
                movableObject.Velocity.X += Math.Sign(obj.DeltaPosition.X)*0.5f;
            }

            // If a vertical collision is detected going downwards, the player has "landed" and he may accelerate
            if (verticalCollision && obj.Velocity.Y > 0)
                obj.Grounded = true;

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

        private void HandleGridCollisions(MovableObject obj)
        {
            var neighboringBlocks = grid.Neighbors(obj);
            var centerBlock = neighboringBlocks[4];

            bool verticalCollision = false;
            bool horizontalCollision = false;

            // Vertical collision check
            // Check blocks 0-2 if going up
            // Check blocks 6-8 if going down
            if (obj.DeltaPosition.Y > 0)
            {
                // Going down
                if (neighboringBlocks[7].Occupied || neighboringBlocks[7 + Math.Sign(obj.Position.X - centerBlock.Position.X)].Occupied)
                {
                    // Check if collision emminent (the 4-index is intentional)
                    if (obj.DeltaPosition.Y > neighboringBlocks[4].Position.Y - obj.Position.Y)
                    {
                        // Cap vertical movement.
                        obj.DeltaPosition.Y = neighboringBlocks[4].Position.Y - obj.Position.Y;
                        verticalCollision = true;
                    }
                }
            }
            else if (obj.DeltaPosition.Y < 0)
            {
                // Going up
                if (neighboringBlocks[1].Occupied || neighboringBlocks[1 + Math.Sign(obj.Position.X - centerBlock.Position.X)].Occupied)
                {
                    // Check if collision emminent
                    if (-obj.DeltaPosition.Y + obj.Size.Y > obj.Position.Y - neighboringBlocks[1].Position.Y)
                    {
                        // Cap vertical movement.
                        obj.DeltaPosition.Y = -(obj.Position.Y - obj.Size.Y - neighboringBlocks[1].Position.Y);
                        verticalCollision = true;
                    }
                }
            }

            // Horizontal collision check
            // Check blocks 0,3,6 if going left
            // Check blocks 2,5,8 if going right
            // If vertical collision detected, then check only 3 and 5
            if (obj.DeltaPosition.X < 0)
            {
                // Going left
                if (neighboringBlocks[3].Occupied || (!verticalCollision && neighboringBlocks[3 + 3 * Math.Sign(obj.Position.Y - centerBlock.Position.Y)].Occupied))
                {
                    // Check if collision emminent (the 4-index is intentional)
                    if (-obj.DeltaPosition.X > obj.Position.X - neighboringBlocks[4].Position.X)
                    {
                        // Cap horizontal movement.
                        obj.DeltaPosition.X = -(obj.Position.X - neighboringBlocks[4].Position.X);
                        horizontalCollision = true;
                    }
                }
            }
            else if (obj.DeltaPosition.X > 0)
            {
                // Going right
                if (neighboringBlocks[5].Occupied || (!verticalCollision && neighboringBlocks[5 + 3 * Math.Sign(obj.Position.Y - centerBlock.Position.Y)].Occupied))
                {
                    // Check if collision emminent
                    if (obj.DeltaPosition.X + obj.Size.X > neighboringBlocks[5].Position.X - obj.Position.X)
                    {
                        // Cap vertical movement.
                        obj.DeltaPosition.X = neighboringBlocks[5].Position.X - obj.Position.X - obj.Size.X;
                        horizontalCollision = true;
                    }
                }
            }

            if (verticalCollision || horizontalCollision)
            {
                var collision = new Collision()
                {
                    Force = Vector2.Zero,
                    HorizontalCollision = horizontalCollision,
                    VerticalCollision = verticalCollision
                };
                obj.OnCollision(null, collision);
            }

            // If a vertical collision is detected going downwards, the player has "landed" and he may accelerate
            if (verticalCollision && obj.Velocity.Y > 0)
                obj.Grounded = true;

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

        public void ApplyToProjectile(Projectile projectile)
        {
            projectile.DecreaseLifespan();
            projectile.DeltaPosition = projectile.Velocity*timestep;
            projectile.Position += projectile.DeltaPosition;

            var neighbors = grid.Neighbors(projectile);
            if (neighbors[4].Occupied)
            {
                var collision = new Collision()
                {
                    Force = projectile.DeltaPosition,
                    HorizontalCollision = true,
                    VerticalCollision = false
                };
                projectile.OnCollision(neighbors[5], collision);
                return;
            }

            foreach (var movableObject in movableObjects)
            {
                if (movableObject == projectile.Shooter)
                    continue;

                if (
                    projectile.Position.X > movableObject.Position.X &&
                    projectile.Position.X < movableObject.Position.X + movableObject.Size.X &&
                    projectile.Position.Y > movableObject.Position.Y &&
                    projectile.Position.Y < movableObject.Position.Y + movableObject.Size.Y)
                {
                    var collision = new Collision()
                    {
                        Force = movableObject.DeltaPosition,
                        HorizontalCollision = true,
                        VerticalCollision = false
                    };
                    projectile.OnCollision(movableObject, collision);
                    movableObject.OnHit(projectile);
                    return;
                }
            }
        }
    }
}
