using System.IO;

namespace CraftingGame
{
    public interface IStreamProvider
    {
        bool FileExists(string path);
        Stream ReadFile(string path);
        Stream WriteFile(string path);
    }
}