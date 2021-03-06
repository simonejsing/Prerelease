﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftingGame.Physics;
using CraftingGame.State;
using VectorMath;

namespace CraftingGame.Controllers
{
    internal class PlayerController
    {
        private readonly GameState state;
        private readonly PhysicsEngine physicsEngine;

        public event EventHandler<PlayerGameStateEvent> PlayerActivated;
        public event EventHandler<PlayerGameStateEvent> PlayerDeactivated;

        public PlayerController(GameState state, PhysicsEngine physicsEngine)
        {
            this.state = state;
            this.physicsEngine = physicsEngine;
        }

        public void Update(PlayerObject player)
        {
            var inputMask = player.InputMask;

            player.Active = inputMask.Input.Active;
            if (inputMask.InputToggled(i => i.Active))
            {
                if (player.Active)
                {
                    PlayerActivated?.Invoke(this, new PlayerGameStateEvent(player));
                }
                else
                {
                    PlayerDeactivated?.Invoke(this, new PlayerGameStateEvent(player));
                }
            }

            if (!player.Active)
            {
                return;
            }

            bool horizontalInput = false;

            var instantVelocity = Vector2.Zero;

            player.Acceleration = Vector2.Zero;

            if (inputMask.Input.Restart)
            {
                SpawnPlayer(player);
            }

            if (player.Dead)
                return;

            if (inputMask.InputToggled(i => i.CycleNextEquipment, true))
            {
                player.EquipNextItem();
            }
            else if (inputMask.InputToggled(i => i.CyclePreviousEquipment, true))
            {
                player.EquipPreviousItem();
            }

            // Update look direction according to inputs
            player.LookDirection = new Vector2();

            var horizontalControl = player.Grounded ? Constants.GROUND_ACCELERATION : Constants.AIR_ACCELERATION;
            if (inputMask.Input.Left)
            {
                horizontalInput = true;
                player.Acceleration -= new Vector2(horizontalControl, 0);
                player.Facing = new Vector2(-1, 0);
                player.LookDirection += new Vector2(-1, 0);
            }
            if (inputMask.Input.Right)
            {
                horizontalInput = true;
                player.Acceleration += new Vector2(horizontalControl, 0);
                player.Facing = new Vector2(1, 0);
                player.LookDirection += new Vector2(1, 0);
            }
            if (inputMask.Input.Up)
            {
                player.LookDirection += new Vector2(0, 1);
            }
            if (inputMask.Input.Down)
            {
                player.LookDirection += new Vector2(0, -1);
            }

            // If player is on the ground and not moving, come to a complete horizontal stop to prevent drift
            if (player.Grounded && !horizontalInput)
            {
                player.Acceleration = new Vector2(0.0f, player.Acceleration.Y);
                player.Velocity = new Vector2(0.0f, player.Velocity.Y);
            }

            player.EquipedItem.Update();

            if (player.Grounded)
            {
                if (inputMask.Input.Jump)
                {
                    instantVelocity = new Vector2(0, Constants.JUMP_SPEED);
                }
            }

            if (inputMask.Input.Attack && !player.EquipedItem.OnCooldown)
            {
                player.EquipedItem.Attack();
            }

            inputMask.Reset();

            physicsEngine.ApplyToObject(player, instantVelocity);

            // Check if player is touching any collectable objects
            // TODO: Quad-tree to partition space such that we don't have to test intersection with every single collectable object on each frame.
            foreach (var touchedObject in state.ActiveLevel.CollectableObjects.Where(o => o.BoundingBox.Intersects(player.BoundingBox)))
            {
                touchedObject.OnCollect(player);
            }
        }

        public void SpawnPlayer(PlayerObject player)
        {
            player.Acceleration = Vector2.Zero;
            player.Velocity = Vector2.Zero;
            player.EquipedItem.Reset();
            player.HitPoints = 1;
            player.Position = new Vector2(state.ActiveLevel.SpawnPoint);
        }
    }
}
