using Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorMath;

namespace Serialization
{
    public class StatefulObject
    {
        private readonly IDictionary<string, object> state;

        public ActionQueue ActionQueue { get; }
        public Guid Id { get; }

        public StatefulObject(ActionQueue actionQueue, Guid key, IDictionary<string, object> state)
        {
            this.ActionQueue = actionQueue;
            this.Id = key;
            this.state = state;
        }

        public T ReadMandatoryState<T>(string propertyName)
        {
            return (T)Convert.ChangeType(state[propertyName], typeof(T));
        }

        public Vector2 SafeReadVector(string propertyName, Vector2? defaultValue = null)
        {
            var def = defaultValue ?? Vector2.Zero;
            return new Vector2(
                SafeReadValue($"{propertyName}:x", def.X),
                SafeReadValue($"{propertyName}:y", def.Y));
        }

        public Color SafeReadColor(string propertyName, Color? defaultValue = null)
        {
            var def = defaultValue ?? Color.Black;
            return new Color(
                SafeReadValue($"{propertyName}:r", def.r),
                SafeReadValue($"{propertyName}:g", def.g),
                SafeReadValue($"{propertyName}:b", def.b),
                SafeReadValue($"{propertyName}:a", def.a));
        }

        public static IDictionary<string, object> EncodeVector(string propertyName, Vector2 vector)
        {
            return new Dictionary<string, object>
            {
                { $"{propertyName}:x", vector.X },
                { $"{propertyName}:y", vector.Y },
            };
        }

        public static IDictionary<string, object> EncodeColor(string propertyName, Color color)
        {
            return new Dictionary<string, object>
            {
                { $"{propertyName}:r", color.r },
                { $"{propertyName}:g", color.g },
                { $"{propertyName}:b", color.b },
                { $"{propertyName}:a", color.a },
            };
        }

        public StatefulObject ReadEmbeddedState(string propertyName)
        {
            var subState = SafeReadEmbeddedState(propertyName, new Dictionary<string, object>());
            return new StatefulObject(this.ActionQueue, this.Id, subState);
        }

        public T SafeReadValue<T>(string propertyName, T defaultValue)
        {
            return SafeOperation(propertyName, o => (T)Convert.ChangeType(o, typeof(T)), defaultValue);
        }

        public IList<StatefulObject> SafeReadList(string propertyName)
        {
            var list = SafeOperation(propertyName, o => ((JArray)o).ToObject<List<IDictionary<string, object>>>(), new List<IDictionary<string, object>>());
            return list.Select(i => new StatefulObject(this.ActionQueue, this.Id, i)).ToList();
        }

        private IDictionary<string, object> SafeReadEmbeddedState(string propertyName, IDictionary<string, object> defaultValue)
        {
            return SafeOperation(propertyName, o => ((JObject)o).ToObject<Dictionary<string, object>>(), defaultValue);
        }

        private T SafeOperation<T>(string propertyName, Func<object, T> operation, T defaultValue)
        {
            if (!state.ContainsKey(propertyName))
            {
                return defaultValue;
            }

            try
            {
                return operation(state[propertyName]);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
