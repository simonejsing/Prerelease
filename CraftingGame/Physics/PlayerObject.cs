using System;
using Contracts;
using VectorMath;
using Serialization;
using System.Collections.Generic;
using CraftingGame.Items;
using System.Linq;
using CraftingGame.Items.Creatable;

namespace CraftingGame.Physics
{
    public class PlayerObject : MovableObject, ICollectingObject
    {
        private Inventory inventory;
        private IEquipableItem[] equipment = new IEquipableItem[0];
        private int equipIndex = 0;

        public string PlayerBinding { get; set; }
        public InputMask InputMask { get; private set; }
        public bool InputBound { get; private set; }
        public bool Active { get; set; }

        public Vector2 LookDirection { get; set; }

        public IEquipableItem EquipedItem => equipIndex < equipment.Length ? equipment[equipIndex] : new NoopItem();
        public int HitPoints { get; set; }
        public bool Dead => HitPoints <= 0;

        public IInventory Inventory => inventory;
        public object Target { get; internal set; }

        public PlayerObject(ActionQueue actionQueue) : base(actionQueue)
        {
            inventory = new Inventory(100);
            InputBound = false;
            HitPoints = 1;
            ObjectCollision += OnObjectCollision;
        }

        public void EquipNextItem()
        {
            equipIndex = (equipIndex + 1) % equipment.Length;
        }

        public void EquipPreviousItem()
        {
            equipIndex--;
            if (equipIndex < 0)
            {
                equipIndex = Math.Max(0, equipment.Length - 1);
            }
        }

        public void SelectEquipmentByName(string name)
        {
            for(var i = 0; i < equipment.Length; i++)
            {
                if(equipment[i].Name == name)
                {
                    equipIndex = i;
                    break;
                }
            }
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
            this.inventory = Physics.Inventory.FromState(state.ReadEmbeddedState("p.inventory"));
        }

        public override void ExtractState(StatefulObjectBuilder builder)
        {
            base.ExtractState(builder);
            builder.Add("p.bind", PlayerBinding);
            builder.EmbedState("p.inventory", inventory);
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

        public void AddEquipment(IEquipableItem item)
        {
            item.Equip(this);
            equipment = equipment.Concat(new[] { item }).ToArray();
        }
    }
}
