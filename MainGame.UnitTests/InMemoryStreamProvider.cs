using System.IO;
using CraftingGame;

namespace MainGame.UnitTests
{
    internal class InMemoryStreamProvider : IStreamProvider
    {
        public bool FileExists(string path)
        {
            return false;
        }

        public Stream ReadFile(string path)
        {
            return new MemoryStream();
        }

        public Stream WriteFile(string path)
        {
            return new MemoryStream();
        }
    }
}