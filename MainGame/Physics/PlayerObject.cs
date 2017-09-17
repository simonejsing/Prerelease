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

        public int ProjectileCount { get; set; }

        public PlayerObject(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size, Color color) : base(actionQueue, startingPosition, size)
        {
            Weapon = new Weapon();
            Active = false;
            Color = color;
            ProjectileCount = 0;
        }
    }
}
