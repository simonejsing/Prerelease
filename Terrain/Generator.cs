using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class Generator : ITerrainGenerator
    {
        private readonly Random random;

        private readonly double BedrockPhase;
        private readonly double RockPhase;

        public int MaxDepth { get; }

        public Generator(int maxDepth, int seed = 0)
        {
            MaxDepth = maxDepth;
            random = new Random(seed);

            BedrockPhase = random.NextDouble() * 2 * Math.PI;
            RockPhase = random.NextDouble() * 2 * Math.PI;
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

        private TerrainType GenerateBlock(int x, int y, int z)
        {
            if(y < -MaxDepth)
                return TerrainType.Free;

            // The depth (y) determines what type of block it is
            var bedrockHeight = BedrockHeight(x);
            if(y <= bedrockHeight)
                return TerrainType.Bedrock;

            var rockHeight = RockHeight(x);
            if (y <= bedrockHeight + rockHeight)
                return TerrainType.Rock;

            if (y <= 0)
                return TerrainType.Dirt;

            return TerrainType.Free;
        }

        private double RockHeight(int x)
        {
            return 1 + Noise(x, 1.0, amplitude: 140.0, phase: RockPhase);
        }

        private double BedrockHeight(int x)
        {
            return 1 + Noise(x, 1.0, amplitude: 20.0, phase: BedrockPhase);
        }

        private static double Noise(double x, double frequency, double amplitude, double phase)
        {
            return amplitude * Math.Sin(2*Math.PI*frequency*x/600.0 + phase);
        }
    }
}
