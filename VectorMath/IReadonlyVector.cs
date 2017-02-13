using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public interface IReadonlyVector
    {
        float X { get; }
        float Y { get; }
    }
}
