using System;
using System.Collections.Generic;
using System.IO;
using CraftingGame;

namespace MainGame.UnitTests
{
    internal class ShadowStream : MemoryStream
    {
        private readonly Action<MemoryStream> closeHook;

        internal ShadowStream(Action<MemoryStream> closeHook)
        {
            this.closeHook = closeHook;
        }

        public override void Close()
        {
            closeHook(this);
            base.Close();
        }
    }

    internal class InMemoryStreamProvider : IStreamProvider
    {
        private Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

        public bool FileExists(string path)
        {
            return files.ContainsKey(path);
        }

        public Stream ReadFile(string path)
        {
            return new MemoryStream(files[path]);
        }

        public Stream WriteFile(string path)
        {
            return new ShadowStream(s => files[path] = s.ToArray());
        }
    }
}