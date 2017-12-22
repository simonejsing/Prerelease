using System;
using System.IO;
using CraftingGame;
using Windows.Storage;

namespace Prerelease.Main
{
    internal class StreamProvider : IStreamProvider
    {
        private static string RootPath = ApplicationData.Current.LocalFolder.Path;

        public bool FileExists(string path)
        {
            return File.Exists(Path.Combine(RootPath, path));
        }

        public Stream ReadFile(string path)
        {
            return ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(path).Result;
        }

        public Stream WriteFile(string path)
        {
            return ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(path, CreationCollisionOption.ReplaceExisting).Result;
        }
    }
}