using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Serialization
{
    public class SerializableState
    {
        public Dictionary<string, Dictionary<Guid, IDictionary<string, object>>> State { get; set; }

        public static SerializableState FromStream(Stream stream)
        {
            using(var reader = new StreamReader(stream))
            {
                var state = JsonConvert.DeserializeObject<SerializableState>(reader.ReadToEnd());
                if(state == null)
                {
                    throw new FileLoadException("Failed to read game state from file.");
                }

                return state;
            }
        }

        public SerializableState()
        {
            this.State = new Dictionary<string, Dictionary<Guid, IDictionary<string, object>>>();
        }

        public void AddEntities(string entityType, IEnumerable<IStatefulEntity> entities)
        {
            State.Add(entityType, entities.ToDictionary(e => e.Id, e => e.ExtractState()));
        }

        public void Serialize(Stream stream)
        {
            using(var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                writer.Write(JsonConvert.SerializeObject(this));
            }
        }
    }
}
