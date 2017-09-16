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

        public PlayerObject(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size, Color color) : base(actionQueue, startingPosition, size)
        {
            Active = false;
            Color = color;
        }
    }
}
