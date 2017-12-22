using Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using VectorMath;

namespace CraftingGame.Physics
{
    internal class StatefulObject
    {
        private readonly IDictionary<string, object> state;

        public ActionQueue ActionQueue { get; }
        public Guid Id { get; }

        public StatefulObject(ActionQueue actionQueue, Guid key, IDictionary<string, object> state)
        {
            this.state = state;
        }

        public T ReadMandatoryState<T>(string property)
        {
            return (T)state[property];
        }

        public Vector2 SafeReadVector(string property, Vector2? defaultValue = null)
        {
            var def = defaultValue ?? Vector2.Zero;
            return new Vector2(
                SafeReadValue($"{property}.x", def.X),
                SafeReadValue($"{property}.y", def.Y));
        }

        internal Color SafeReadColor(string property, Color? defaultValue = null)
        {
            var def = defaultValue ?? Color.Black;
            return new Color(
                SafeReadValue($"{property}.r", def.r),
                SafeReadValue($"{property}.g", def.g),
                SafeReadValue($"{property}.b", def.b),
                SafeReadValue($"{property}.a", def.a));
        }

        public T SafeReadValue<T>(string property, T defaultValue)
        {
            if (!state.ContainsKey(property))
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(state[property], typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
