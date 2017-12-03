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
            var samples = MountainTerrain();
            //var samples = HillTerrain();

            var content = "x,y" + Environment.NewLine + string.Join(Environment.NewLine, samples.Select(s => s.Item1 + "," + s.Item2));
            File.WriteAllText("noise.csv", content);
        }

        private static Tuple<double, double>[] HillTerrain()
        {
            // Noise parameters
            const double frequency = 0.5 / 32.0;
            const double amplitude = 1.0;
            const double exponent = 7.0;
            const int octaves = 4;
            const double persistence = 0.5;

            // Sample area
            const double intervalStart = 10.0; // Phase shift
            const double intervalLength = 1000.0;
            const int numberOfSamples = 500;

            var generator = new OctaveNoise(new PerlinNoise(0), octaves, persistence);
            var samples = new Tuple<double, double>[numberOfSamples];
            for (int sample = 0; sample < numberOfSamples; sample++)
            {
                var x = intervalLength / numberOfSamples * sample;
                //var value = noise.OctavePerlin(x, 0.5, 7, 0.5, 1); // Looks a bit like mountains
                //var value = noise.OctavePerlin(x, 0.5, 2, 0.5, 1); // Looks like smooth hills
                var value = Math.Pow(generator.Noise(x, 0.5, amplitude, frequency, intervalStart, 1.0), exponent) /
                            Math.Pow(amplitude, exponent);
                //var value = 10.0*Math.Pow(noise.OctavePerlin(x, 0.5, 1, 2.0, 2), 8.0); // Mountain peaks
                //var value = noise.perlin(x, 0.5);
                samples[sample] = new Tuple<double, double>(x, value);
            }
            return samples;
        }

        private static Tuple<double, double>[] MountainTerrain()
        {
            // Noise parameters
            const double frequency = 0.5/32.0;
            const double amplitude = 1.0;
            const double exponent = 7.0;
            const int octaves = 4;
            const double persistence = 0.5;

            // Sample area
            const double intervalStart = 10.0; // Phase shift
            const double intervalLength = 1000.0;
            const int numberOfSamples = 500;

            var generator = new OctaveNoise(new PerlinNoise(0), octaves, persistence);
            var samples = new Tuple<double, double>[numberOfSamples];
            for (int sample = 0; sample < numberOfSamples; sample++)
            {
                var x = intervalLength/numberOfSamples*sample;
                //var value = noise.OctavePerlin(x, 0.5, 7, 0.5, 1); // Looks a bit like mountains
                //var value = noise.OctavePerlin(x, 0.5, 2, 0.5, 1); // Looks like smooth hills
                var value = Math.Pow(generator.Noise(x, 0.5, amplitude, frequency, intervalStart, 1.0), exponent)/
                            Math.Pow(amplitude, exponent);
                //var value = 10.0*Math.Pow(noise.OctavePerlin(x, 0.5, 1, 2.0, 2), 8.0); // Mountain peaks
                //var value = noise.perlin(x, 0.5);
                samples[sample] = new Tuple<double, double>(x, value);
            }
            return samples;
        }
    }
}
