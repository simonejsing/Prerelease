using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using CraftingGame.State;
using Serialization;
using VectorMath;

namespace CraftingGame.Physics
{
    public class Object : IRenderableObject, IStatefulEntity
    {
        public ActionQueue ActionQueue { get; }
        public Guid Id { get; set; }

        public Rect2 BoundingBox { get; private set; }

        public Plane Plane { get; set; }
        public Vector2 Position
        {
            get { return BoundingBox.TopLeft; }
            set { BoundingBox = new Rect2(value, BoundingBox.Size); }
        }
        public Vector2 Facing { get; set; }

        public Vector2 Size
        {
            get { return BoundingBox.Size; }
            set { BoundingBox = new Rect2(BoundingBox.TopLeft, value); }
        }

        public Color Color { get; set; }

        public IBinding<ISprite> SpriteBinding { get; set; }
        public GameAction Action { get; set; }

        public Vector2 Center => Position + 0.5f * Size;

        public Object(ActionQueue actionQueue)
        {
            this.ActionQueue = actionQueue;
            this.BoundingBox = new Rect2();
            this.Facing = new Vector2(1, 0);
            this.Action = new GameAction(ActionType.Noop);
        }

        public void Activate(Object activator)
        {
            ActionQueue.Enqueue(Action);
        }

        protected virtual void Load(StatefulObject state)
        {
            this.SpriteBinding = new ObjectBinding<ISprite>(state.ReadMandatoryState<string>("o.sprite"));
            this.Plane = new Plane(state.SafeReadValue("o.pl", 0));
            this.Position = state.SafeReadVector("o.p");
            this.Size = state.SafeReadVector("o.s");
            this.Facing = state.SafeReadVector("o.f", new Vector2(1, 0));
            this.Color = state.SafeReadColor("o.c");
        }

        public virtual void ExtractState(StatefulObjectBuilder builder)
        {
            builder.Add("o.sprite", SpriteBinding.Path);
            builder.Add("o.pl", Plane.W);
            builder.EncodeVector("o.p", Position);
            builder.EncodeVector("o.s", Size);
            builder.EncodeVector("o.f", Facing);
            builder.EncodeColor("o.c", Color);
        }
    }
}