using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class Generator : ITerrainGenerator
    {
        private readonly INoiseGenerator rockGenerator, dirtGenerator, seabedGenerator;

        public int SeaLevel { get; }
        public int MaxDepth { get; }
        public int MaxHeight { get; }

        public Generator(int maxDepth, int maxHeight, int seaLevel, int seed = 0)
        {
            SeaLevel = seaLevel;
            MaxDepth = maxDepth;
            MaxHeight = maxHeight;
            var noise = new PerlinNoise(seed);

            rockGenerator = new OctaveNoise(noise, 4, 0.5);
            dirtGenerator = new OctaveNoise(noise, 4, 0.8);
            seabedGenerator = new OctaveNoise(noise, 4, 0.08);
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
            var dy = y + MaxDepth;
            var bedrockHeight = BedrockLevel(x, z + 0.5); // Offset by 0.5 for more randomness
            var rockHeight = bedrockHeight + RockLevel(x, z + 0.5); // Offset by 0.5 for more randomness
            var dirtHeight = DirtLevel(x, z + 0.5); // Offset by 0.5 for more randomness

            if (dy <= bedrockHeight)
                return TerrainType.Bedrock;

            if (dy <= rockHeight)
                return TerrainType.Rock;

            if (rockHeight <= SeaLevel)
            {
                if (dy <= SeaLevel)
                    return TerrainType.Sea;

                return TerrainType.Free;
            }

            if (dy <= dirtHeight)
                return TerrainType.Dirt;

            return TerrainType.Free;
        }

        public double BedrockLevel(double x, double y)
        {
            // Noise parameters
            const double frequency = 1.0 / 512.0;
            const double amplitude = 1.0;
            const double exponent = 2.0;
            const double scale = 2.0;
            const double phase = 10.0;
            const double damping = 6.0;

            var value = Clamp(
                scale * (Math.Pow(rockGenerator.Noise(x, y, amplitude, frequency, phase, damping), exponent) / Math.Pow(amplitude, exponent) - 0.2),
                0.0,
                1.0);

            return ScaleLevel(value);
        }

        public double SeabedLevel(double x, double y)
        {
            // Noise parameters
            const double frequency = 1.0 / 4096.0;
            const double amplitude = 1.0;
            const double exponent = 7.0;
            const double damping = 1.0;
            const double phase = 10.0;

            var value = 10.0 * Math.Pow(seabedGenerator.Noise(x, 0.5, amplitude, frequency, phase, damping), exponent) - 0.5;
            value = Clamp(1.0 - Softmax(10.0 * value), 0.0, 1.0);
            return value;
        }

        public double RockLevel(double x, double y)
        {
            // Noise parameters
            const double frequency = 1.0 / 256.0;
            const double amplitude = 1.0;
            const double exponent = 7.0;
            const double scale = 8.0;
            const double phase = 10.0;
            const double damping = 1.0;
            const double offset = 1.0 / 4.0;

            var seabed = SeabedLevel(x, y);
            var value = offset + scale * Math.Pow(rockGenerator.Noise(x, y, amplitude, frequency, phase, damping), exponent) / Math.Pow(amplitude, exponent);

            return ScaleLevel(seabed * value);
        }

        public double DirtLevel(double x, double y)
        {
            // Noise parameters
            const double frequency = 1.0 / 512.0;
            const double amplitude = 1.0;
            const double exponent = 1.0;
            const double phase = 10.0;
            const double damping = 4.0;
            const double offset = 0.0;

            var value = offset + Math.Pow(dirtGenerator.Noise(x, y, amplitude, frequency, phase, damping), exponent) /
                        Math.Pow(amplitude, exponent);

            return ScaleLevel(value);
        }

        private double ScaleLevel(double y)
        {
            return Clamp(y, 0.0, 1.0) * (MaxHeight + MaxDepth);
        }

        private static double Softmax(double x)
        {
            return 1.0 / (1 + Math.Exp(-x));
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
