using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public interface IReadonlyRectangle
    {
        IReadonlyPoint Position { get; }
        IReadonlyPoint Size { get; }
    }
}
