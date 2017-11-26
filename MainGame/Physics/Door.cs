using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public enum DestinationType { Exit, Level }

    public struct Destination
    {
        public DestinationType Type;
        public string Identifier;
    }

    public class Door : Object
    {
        public Door(ActionQueue actionQueue, IReadonlyVector startingPosition, IReadonlyVector size) : base(actionQueue, startingPosition, size)
        {
        }

        public Destination Destination { get; set; }
    }
}
