using Contracts;
using CraftingGame.State;
using Serialization;
using System.Collections.Generic;
using VectorMath;

namespace CraftingGame.Physics
{
    public class MovableObject : StaticObject
    {
        public bool Grounded { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 DeltaPosition { get; set; }

        public MovableObject(ActionQueue actionQueue) : base(actionQueue)
        {
            Grounded = false;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            DeltaPosition = Vector2.Zero;

            ObjectCollision += HandleGroundObjectCollision;
            GridCollision += (sender, target, collision) => HandleGroundObjectCollision(sender, target[4], collision);
        }

        private static void HandleGroundObjectCollision(object sender, ICollidableObject target, Collision collision)
        {
            var obj = sender as MovableObject;

            // If a vertical collision is detected going downwards, the player has "landed" and he may accelerate
            if (obj != null && collision.VerticalCollision && obj.Velocity.Y < 0)
            {
                obj.Grounded = true;
            }
        }

        protected override void Load(StatefulObject state)
        {
            base.Load(state);
            this.Acceleration = state.SafeReadVector("m.a");
            this.Velocity = state.SafeReadVector("m.v");
            this.Grounded = state.SafeReadValue("m.g", false);
        }

        public override void ExtractState(StatefulObjectBuilder builder)
        {
            base.ExtractState(builder);
            builder.Add("m.g", this.Grounded);
            builder.EncodeVector("m.a", this.Acceleration);
            builder.EncodeVector("m.v", this.Velocity);
        }
    }
}
