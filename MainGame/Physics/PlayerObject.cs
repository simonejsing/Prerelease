using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class PlayerObject : MovableObject
    {
        public bool Active { get; set; }
        public Color Color { get; }
        public Weapon Weapon { get; }
        public int HitPoints { get; set; }
        public bool Dead => HitPoints <= 0;

        public PlayerObject(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size, Color color) : base(actionQueue, startingPosition, size)
        {
            Weapon = new Weapon();
            Active = false;
            Color = color;
            HitPoints = 1;
            Collision += OnCollision;
        }

        private void OnCollision(object sender, ICollidableObject target, Collision collision)
        {
            // Allow player to push other movable objects
            var obj = target as MovableObject;
            if (obj != null && collision.HorizontalCollision)
            {
                obj.Velocity.X += Math.Sign(collision.Force.X) * 0.5f;
            }
        }
    }
}
