using System;
using System.Linq;
using Contracts;
using Prerelease.Main.Physics;
using VectorMath;

namespace Prerelease.Main
{
    public class PlatformerSceene : Sceene
    {
        private PlayerObject[] players;
        private BlockGrid blocks;
        private ISprite blockSprite;

        private const float Gravity = 0.1f;
        private const float MaxVerticalVelocity = 8.0f;
        private const float MaxHorizontalVelocity = 8.0f;

        public PlatformerSceene(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue)
            : base("Platformer", renderer, ui, actionQueue)
        {
            players = new[]
            {
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Red),
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Green),
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Blue),
                new PlayerObject(actionQueue, Vector2.Zero, new Vector2(30, 30), Color.Yellow),
            };

            blocks = new BlockGrid(30, 30, 100, 100);
            for (uint i = 0; i < 40; i++)
            {
                blocks.Insert(10, i);
                if (i > 15)
                {
                    blocks.Insert(5, i);
                }
            }

            blocks.Insert(9, 8);
            blocks.Insert(9, 39);

            blocks.Insert(8, 10);
            blocks.Insert(7, 11);
            blocks.Insert(6, 12);
            blocks.Insert(5, 13);
            blocks.Insert(4, 14);
            blocks.Insert(3, 15);
        }

        public override void Activate()
        {
            base.Activate();

            // Load content in active scope.
            blockSprite = LoadSprite("Block");
            foreach (var player in players)
            {
                player.Sprite = LoadSprite("Chicken");
                player.Sprite.Size = new Vector2(30, 30);
            }
        }

        public override void Update(float timestep, InputMask[] inputMasks)
        {
            for (var i = 0; i < players.Length; i++)
            {
                HandlePlayerInput(players[i], inputMasks[i], timestep);
            }
        }

        readonly Vector2 instantVelocity = Vector2.Zero;

        private void HandlePlayerInput(PlayerObject player, InputMask inputMask, float timestep)
        {
            player.Active = inputMask.Input.Active;
            if (!inputMask.Input.Active)
                return;

            instantVelocity.Clear();

            player.Acceleration.Clear();

            if (inputMask.Input.Restart)
            {
                player.Position.Clear();
                player.Velocity.Clear();
            }

            if (player.CanAccelerate)
            {
                if (inputMask.Input.Up)
                {
                    instantVelocity.Y = -4;
                }
            }

            var horizontalControl = player.CanAccelerate ? 0.5f : 0.1f;
            if (inputMask.Input.Left)
            {
                player.Acceleration.X -= horizontalControl;
            }
            if (inputMask.Input.Right)
            {
                player.Acceleration.X += horizontalControl;
            }

            if (inputMask.Input.Select)
            {
            }
            inputMask.Reset();

            // Apply gravity
            player.Acceleration.Y += Gravity;

            // Accelerate player
            player.Velocity += instantVelocity + player.Acceleration * timestep;

            // Cap velocity
            if (Math.Abs(player.Velocity.Y) > MaxVerticalVelocity)
            {
                player.Velocity.Y = Math.Sign(player.Velocity.Y) * MaxVerticalVelocity;
            }
            if (Math.Abs(player.Velocity.X) > MaxHorizontalVelocity)
            {
                player.Velocity.X = Math.Sign(player.Velocity.X) * MaxHorizontalVelocity;
            }

            // Handle collisions with block grid
            var deltaPosition = HandleCollisions(player, blocks, timestep);

            // Move player
            player.Position += deltaPosition;
        }

        private ICollidableObject[] neighboringBlocks = null;

        private Vector2 HandleCollisions(MovableObject obj, BlockGrid blocks, float timestep)
        {
            neighboringBlocks = blocks.Neighbors(obj);
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
                if (neighboringBlocks[3].Occupied || (!verticalCollision && neighboringBlocks[3 + 3*Math.Sign(obj.Position.Y - centerBlock.Position.Y)].Occupied))
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
                if (neighboringBlocks[5].Occupied || (!verticalCollision && neighboringBlocks[5 + 3*Math.Sign(obj.Position.Y - centerBlock.Position.Y)].Occupied))
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

        public override void Render(double gameTimeMsec)
        {
            Renderer.Clear(Color.Black);

            foreach (var block in blocks.OccupiedBlocks)
            {
                Renderer.RenderOpagueSprite(blockSprite, block.Position, blocks.GridSize);
            }

            if (neighboringBlocks != null)
            {
                foreach (var block in neighboringBlocks)
                {
                    Renderer.RenderRectangle(block.Position, blocks.GridSize, block.Occupied ? Color.Red : Color.Green);
                }
            }

            foreach (var player in players.Where(p => p.Active))
            {
                Renderer.RenderOpagueSprite(player.Sprite, player.Position, player.Size);
            }
        }
    }
}