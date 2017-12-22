using Serialization;
using Terrain;

namespace CraftingGame
{
    public interface ITerrainFactory
    {
        ITerrainGenerator Create();
        ITerrainGenerator FromState(StatefulObject state);
    }
}