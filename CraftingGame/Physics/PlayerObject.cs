using System;
using Contracts;
using VectorMath;
using Serialization;
using System.Collections.Generic;
using CraftingGame.State;

namespace CraftingGame.Physics
{
    public class PlayerObject : MovableObject, ICollectingObject
    {
        public string PlayerBinding { get; set; }
        public InputMask InputMask { get; private set; }
        public bool InputBound { get; private set; }
        public bool Active { get; set; }

        public Vector2 LookDirection { get; set; }
        public Color Color { get; set; }

        public Weapon Weapon { get; }
        public int HitPoints { get; set; }
        public bool Dead => HitPoints <= 0;

        public IInventory Inventory { get; }

        public PlayerObject(ActionQueue actionQueue) : base(actionQueue)
        {
            Weapon = new Weapon();
            Inventory = new Inventory(100);
            InputBound = false;
            HitPoints = 1;
            ObjectCollision += OnObjectCollision;
        }

        public void BindInput(InputMask inputMask)
        {
            if(inputMask.PlayerBinding != PlayerBinding)
            {
                throw new InvalidOperationException($"Attempt to bind invalid input '{inputMask.PlayerBinding}' to player '{PlayerBinding}'.");
            }
            this.InputMask = inputMask;
            this.InputMask.Bound = true;
            this.InputBound = true;
        }

        private void OnObjectCollision(object sender, ICollidableObject target, Collision collision)
        {
            // Allow player to push other movable objects
            var obj = target as MovableObject;
            if (obj != null && collision.HorizontalCollision)
            {
                obj.Velocity += new Vector2(Math.Sign(collision.Force.X) * 0.5f, 0);
            }
        }

        protected override void Load(StatefulObject state)
        {
            base.Load(state);
            this.PlayerBinding = state.ReadMandatoryState<string>("p.bind");
            this.Color = state.SafeReadColor("p.c");
        }

        public override void ExtractState(StatefulObjectBuilder builder)
        {
            base.ExtractState(builder);
            builder.Add("p.bind", PlayerBinding);
            builder.EncodeColor("p.c", Color);
        }

        public static PlayerObject FromState(StatefulObject state)
        {
            var player = new PlayerObject(state.ActionQueue)
            {
                Id = state.Id,
            };
            player.Load(state);
            return player;
        }
    }
}
