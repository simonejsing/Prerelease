using System;
using Contracts;
using VectorMath;
using Serialization;
using System.Collections.Generic;

namespace CraftingGame.Physics
{
    public class PlayerObject : MovableObject, ICollectingObject, IStatefulEntity
    {
        public string PlayerBinding { get; }
        public InputMask InputMask { get; private set; }
        public bool InputBound { get; private set; }
        public bool Active { get; set; }

        public Vector2 LookDirection { get; set; }
        public Color Color { get; }
        public Weapon Weapon { get; }
        public int HitPoints { get; set; }
        public bool Dead => HitPoints <= 0;

        public IInventory Inventory { get; }

        public Guid Id { get; }

        public PlayerObject(ActionQueue actionQueue, Guid id, string playerBinding, Plane startingPlane, IReadonlyVector startingPosition, IReadonlyVector size, string spritePath, Color color) : base(actionQueue, startingPlane, startingPosition, size)
        {
            this.Id = id;
            Weapon = new Weapon();
            Inventory = new Inventory(100);
            PlayerBinding = playerBinding;
            InputBound = false;
            SpriteBinding = new ObjectBinding<ISprite>(spritePath);
            Color = color;
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

        internal static PlayerObject FromState(StatefulObject state)
        {
            return new PlayerObject(
                state.ActionQueue,
                state.Id,
                state.ReadMandatoryState<string>("bind"),
                new Plane(state.SafeReadValue("pl", 0)),
                state.SafeReadVector("p"),
                state.SafeReadVector("s"),
                state.ReadMandatoryState<string>("sprite"),
                state.SafeReadColor("c"));
        }

        public IDictionary<string, object> ExtractState()
        {
            return new Dictionary<string, object>
            {
                { "bind", PlayerBinding },
                { "sprite", SpriteBinding.Path },
                { "pl", Plane.W },
                { "p.x", Position.X },
                { "p.y", Position.Y },
                { "s.x", Size.X },
                { "s.y", Size.Y },
                { "c.r", Color.r },
                { "c.g", Color.g },
                { "c.b", Color.b },
                { "c.a", Color.a },
            };
        }
    }
}
