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
        }

        protected static IDictionary<string, object> ConcatenateState(IDictionary<string, object> baseState, params IDictionary<string, object>[] states)
        {
            var merged = states.SelectMany(s => s).ToDictionary(d => d.Key, d => d.Value);
            return baseState.Concat(merged).ToDictionary(d => d.Key, d => d.Value);
        }

        public virtual IDictionary<string, object> ExtractState()
        {
            return ConcatenateState(
                new Dictionary<string, object>
                {
                    { "o.sprite", SpriteBinding.Path },
                    { "o.pl", Plane.W },
                },
                StatefulObject.EncodeVector("o.p", Position),
                StatefulObject.EncodeVector("o.s", Size),
                StatefulObject.EncodeVector("o.f", Facing)
            );
        }
    }
}