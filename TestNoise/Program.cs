using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace TestNoise
{
    class Program
    {
        static void Main(string[] args)
        {
            //var samples = MountainTerrain();
            var samples = BedrockTerrain();

            var content = "x,y" + Environment.NewLine + string.Join(Environment.NewLine, samples.Select(s => s.Item1 + "," + s.Item2));
            File.WriteAllText("noise.csv", content);
        }

        private static Tuple<double, double>[] BedrockTerrain()
        {
            // Noise parameters
            const double frequency = 1.0 / 512.0;
            const double amplitude = 1.0;
            const double exponent = 2.0;
            const int octaves = 4;
            const double persistence = 0.8;
            const double damping = 6.0;

            // Sample area
            const double intervalStart = 10.0; // Phase shift
            const double intervalLength = 1000.0;
            const int numberOfSamples = 500;

            var generator = new OctaveNoise(new PerlinNoise(0), octaves, persistence);
            var samples = new Tuple<double, double>[numberOfSamples];
            for (int sample = 0; sample < numberOfSamples; sample++)
            {
                var x = intervalLength / numberOfSamples * sample;
                var value = Clamp(Math.Pow(generator.Noise(x, 0.5, amplitude, frequency, intervalStart, damping), exponent) /
                            Math.Pow(amplitude, exponent) - 0.2, 0.0, 1.0);
                samples[sample] = new Tuple<double, double>(x, value);
            }
            return samples;
        }

        private static Tuple<double, double>[] HillTerrain()
        {
            // Noise parameters
            const double frequency = 1.0 / 512.0;
            const double amplitude = 1.0;
            const double exponent = 1.0;
            const int octaves = 4;
            const double persistence = 0.8;

            // Sample area
            const double intervalStart = 10.0; // Phase shift
            const double intervalLength = 1000.0;
            const int numberOfSamples = 500;

            var generator = new OctaveNoise(new PerlinNoise(0), octaves, persistence);
            var samples = new Tuple<double, double>[numberOfSamples];
            for (int sample = 0; sample < numberOfSamples; sample++)
            {
                var x = intervalLength / numberOfSamples * sample;
                var value = Math.Pow(generator.Noise(x, 0.5, amplitude, frequency, intervalStart, 8.0), exponent) /
                            Math.Pow(amplitude, exponent);
                samples[sample] = new Tuple<double, double>(x, value);
            }
            return samples;
        }

        private static Tuple<double, double>[] MountainTerrain()
        {
            // Noise parameters
            const double frequency = 1.0/256.0;
            const double amplitude = 1.0;
            const double exponent = 7.0;
            const int octaves = 4;
            const double persistence = 0.5;
            const double scale = 8.0;

            // Sample area
            const double intervalStart = 10.0; // Phase shift
            const double intervalLength = 1000.0;
            const int numberOfSamples = 500;

            var generator = new OctaveNoise(new PerlinNoise(0), octaves, persistence);
            var samples = new Tuple<double, double>[numberOfSamples];
            for (int sample = 0; sample < numberOfSamples; sample++)
            {
                var x = intervalLength/numberOfSamples*sample;
                var value = Clamp(scale * Math.Pow(generator.Noise(x, 0.5, amplitude, frequency, intervalStart, 1.0), exponent)/
                            Math.Pow(amplitude, exponent), 0.0, 1.0);
                samples[sample] = new Tuple<double, double>(x, value);
            }
            return samples;
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
