using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftingGame.Physics;
using VectorMath;

namespace CraftingGame.Controllers
{
    public delegate void DigEventHandler(object sender, PlayerObject player);

    internal class PlayerController
    {
        private readonly GameState state;
        private readonly PhysicsEngine physicsEngine;
        private readonly Camera camera;

        public event DigEventHandler Dig;

        public PlayerController(GameState state, PhysicsEngine physicsEngine, Camera camera)
        {
            this.state = state;
            this.physicsEngine = physicsEngine;
            this.camera = camera;
        }

        public void OnDig(PlayerObject player)
        {
            Dig?.Invoke(this, player);
        }

        public void Update(PlayerObject player)
        {
            var inputMask = player.InputMask;

            player.Active = inputMask.Input.Active;
            if (!player.Active)
                return;

            if (inputMask.Input.Moving)
            {
                camera.Follow();
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

            if (player.Grounded)
            {
                if (inputMask.Input.Select)
                {
                    // Enter door if on door sprite (next frame update will carry out the transition
                    state.DoorToEnter = state.ActiveLevel.Doors.FirstOrDefault(d => player.BoundingBox.Intersects(d.Center));
                }
                if (inputMask.Input.Up)
                {
                    instantVelocity = new Vector2(0, Constants.JUMP_SPEED);
                }
                if (inputMask.Input.Fire && player.Weapon.CanFire)
                {
                    player.Weapon.Cooldown = 10;
                    OnDig(player);
                }
            }

            var horizontalControl = player.Grounded ? Constants.GROUND_ACCELERATION : Constants.AIR_ACCELERATION;
            if (inputMask.Input.Left)
            {
                horizontalInput = true;
                player.Acceleration -= new Vector2(horizontalControl, 0);
                player.Facing = new Vector2(-1, 0);
            }
            if (inputMask.Input.Right)
            {
                horizontalInput = true;
                player.Acceleration += new Vector2(horizontalControl, 0);
                player.Facing = new Vector2(1, 0);
            }

            // If player is on the ground and not moving, come to a complete horizontal stop to prevent drift
            if (player.Grounded && !horizontalInput)
            {
                player.Acceleration = new Vector2(0.0f, player.Acceleration.Y);
                player.Velocity = new Vector2(0.0f, player.Velocity.Y);
            }

            if (inputMask.Input.Fire && player.Weapon.CanFire)
            {
                //state.ActiveLevel.AddProjectiles(FireWeapon(player));
            }

            player.Weapon.Update();

            inputMask.Reset();

            physicsEngine.ApplyToObject(player, instantVelocity);

            // Check if player is touching any collectable objects
            // TODO: Quad-tree to partition space such that we don't have to test intersection with every single collectable object on each frame.
            foreach (var touchedObject in state.ActiveLevel.CollectableObjects.Where(o => o.BoundingBox.Intersects(player.BoundingBox)))
            {
                touchedObject.OnCollect(player);
                touchedObject.PickedUp = true;
            }
        }

        public void SpawnPlayer(PlayerObject player)
        {
            player.Acceleration = Vector2.Zero;
            player.Velocity = Vector2.Zero;
            player.Weapon.Cooldown = 0;
            player.HitPoints = 1;
            player.Position = new Vector2(state.ActiveLevel.SpawnPoint);
        }

        private Projectile FireWeapon(PlayerObject player)
        {
            player.Weapon.Cooldown = 10;
            return new Projectile(player.ActionQueue, player, player.Color, player.Center, new Vector2(Constants.PROJECTILE_VELOCITY * player.Facing.X, 0.0f), new Vector2(1, 1));
        }
    }
}
