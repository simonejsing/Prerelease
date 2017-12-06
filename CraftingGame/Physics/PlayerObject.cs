﻿using System;
using Contracts;
using VectorMath;

namespace CraftingGame.Physics
{
    public class PlayerObject : MovableObject, ICollectingObject
    {
        public InputMask InputMask { get; }

        public bool Active { get; set; }
        public Color Color { get; }
        public Weapon Weapon { get; }
        public int HitPoints { get; set; }
        public bool Dead => HitPoints <= 0;

        public IInventory Inventory { get; }

        public PlayerObject(ActionQueue actionQueue, InputMask inputMask, IReadonlyVector startingPosition, IReadonlyVector size, string spritePath, Color color) : base(actionQueue, startingPosition, size)
        {
            Weapon = new Weapon();
            Inventory = new Inventory();
            InputMask = inputMask;
            Active = false;
            SpriteBinding = new ObjectBinding<ISprite>(spritePath);
            Color = color;
            HitPoints = 1;
            ObjectCollision += OnObjectCollision;
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
    }
}
