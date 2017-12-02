﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class Generator
    {
        public enum TerrainType { Free = 0, Dirt = 1, Rock = 2, Bedrock = 3 }

        public struct TerrainBlock
        {
            public int X, Y;
            public TerrainType Type;
        }

        public TerrainBlock this[int x, int y, int z]
        {
            get
            {
                var type = GenerateBlock(x, y, z);

                return new TerrainBlock()
                {
                    X = x,
                    Y = y,
                    Type = type,
                };
            }
        }

        private static TerrainType GenerateBlock(int x, int y, int z)
        {
            if(x + y == 0)
                return TerrainType.Rock;
            return y > 0 ? TerrainType.Free : TerrainType.Dirt;
        }
    }
}
