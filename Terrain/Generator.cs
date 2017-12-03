using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class Generator : ITerrainGenerator
    {
        private readonly INoiseGenerator rockGenerator, dirtGenerator;

        public int MaxDepth { get; }
        public int MaxHeight { get; }

        public Generator(int maxDepth, int maxHeight, int seed = 0)
        {
            MaxDepth = maxDepth;
            MaxHeight = maxHeight;
            var noise = new PerlinNoise(seed);

            rockGenerator = new OctaveNoise(noise, 4, 0.5);
            dirtGenerator = new OctaveNoise(noise, 4, 0.8);
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
            if(y < -MaxDepth || y > MaxHeight)
                return TerrainType.Free;

            // The depth (y) determines what type of block it is
            /*var bedrockHeight = BedrockHeight(x);
            if(y <= bedrockHeight)
                return TerrainType.Bedrock;*/

            var rockHeight = RockLevel(x, z + 0.5); // Offset by 0.5 for more randomness
            var dirtHeight = DirtLevel(x, z + 0.5); // Offset by 0.5 for more randomness
            var maxHeight = Math.Max(rockHeight, dirtHeight);

            if (y > maxHeight)
                return TerrainType.Free;

            if (dirtHeight > rockHeight)
                return TerrainType.Dirt;

            return TerrainType.Rock;
        }

        private double RockLevel(double x, double y)
        {
            // Noise parameters
            const double frequency = 1.0 / 256.0;
            const double amplitude = 1.0;
            const double exponent = 7.0;
            const double scale = 8.0;
            const double phase = 10.0;
            const double damping = 1.0;

            var value = Clamp(
                scale * Math.Pow(rockGenerator.Noise(x, y, amplitude, frequency, phase, damping), exponent) / Math.Pow(amplitude, exponent),
                0.0,
                1.0);

            return ScaleLevel(value);
        }

        private double DirtLevel(double x, double y)
        {
            // Noise parameters
            const double frequency = 1.0 / 512.0;
            const double amplitude = 1.0;
            const double exponent = 1.0;
            const double phase = 10.0;
            const double damping = 8.0;

            var value = Math.Pow(dirtGenerator.Noise(x, y, amplitude, frequency, phase, damping), exponent) /
                        Math.Pow(amplitude, exponent);

            return ScaleLevel(value);
        }

        private double ScaleLevel(double y)
        {
            return y * (MaxHeight + MaxDepth) - MaxDepth;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }
}
