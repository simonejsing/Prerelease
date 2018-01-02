using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorMath;

namespace MainGame.UnitTests
{
    internal class GpuTextureStub : IGpuTexture
    {
        public Vector2 Size { get; }
        public bool ContentLost => false;

        public static IGpuTexture Create(int width, int height)
        {
            return new GpuTextureStub(width, height);
        }

        public GpuTextureStub(int width, int height)
        {
            Size = new Vector2(width, height);
        }
    }
}
