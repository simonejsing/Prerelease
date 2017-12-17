using System.Collections.Generic;
using CraftingGame.Physics;

namespace CraftingGame
{
    public interface IObjectManager
    {
        ICollidableObjectGrid Blocks { get; }
        IEnumerable<ICollidableObject> CollisionOrder { get; }
    }
}