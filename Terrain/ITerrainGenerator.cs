namespace Terrain
{
    public interface ITerrainGenerator
    {
        int SeaLevel { get; }
        int MaxDepth { get; }
        int MaxHeight { get; }

        TerrainBlock this[int x, int y, int z] { get; }

        void Generate(int x, int y, int z);
    }
}