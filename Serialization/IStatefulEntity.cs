using System;
using System.Collections.Generic;
using System.Text;

namespace Serialization
{
    public interface IStatefulEntity
    {
        Guid Id { get; }
        IDictionary<string, object> ExtractState();
    }
}
