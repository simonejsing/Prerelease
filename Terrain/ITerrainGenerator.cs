namespace Terrain
{
    public interface ITerrainGenerator
    {
        TerrainBlock this[int x, int y, int z] { get; }
    }
}