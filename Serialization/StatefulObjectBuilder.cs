using Contracts;
using System;
using System.Collections.Generic;
using VectorMath;

namespace Serialization
{
    public class StatefulObjectBuilder
    {
        public IDictionary<string, object> State { get; } = new Dictionary<string, object>();

        public StatefulObjectBuilder()
        {
        }

        public void EncodeVector(string propertyName, Vector2 vector)
        {
            Add($"{propertyName}:x", vector.X);
            Add($"{propertyName}:y", vector.Y);
        }

        public void EncodeColor(string propertyName, Color color)
        {
            Add($"{propertyName}:r", color.r);
            Add($"{propertyName}:g", color.g);
            Add($"{propertyName}:b", color.b);
            Add($"{propertyName}:a", color.a);
        }

        public void Add(string propertyName, object value)
        {
            State.Add(propertyName, value);
        }

        public void EmbedState(string propertyName, IStatefulEntity entity)
        {
            var builder = new StatefulObjectBuilder();
            entity.ExtractState(builder);
            State.Add(propertyName, builder.State);
        }
    }
}