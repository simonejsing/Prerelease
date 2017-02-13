using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorMath;

namespace Contracts
{
    public interface ISprite
    {
        IReadonlyRectangle SourceRectangle { get; }
        Vector2 Size { get; set; }
    }
}
