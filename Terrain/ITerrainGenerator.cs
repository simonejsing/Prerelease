using Contracts;

namespace Terrain
{
    public interface ITerrainGenerator
    {
        int SeaLevel { get; }
        int MaxDepth { get; }
        int MaxHeight { get; }

        TerrainBlock this[Coordinate c, Plane p] { get; }

        void Generate(Coordinate c, Plane p);
    }
}