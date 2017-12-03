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
            PerlinNoise noise = new PerlinNoise(0);

            var intervalStart = 0.0;
            var intervalLength = 10.0;
            const int NumberOfSamples = 500;
            var samples = new Tuple<double, double>[NumberOfSamples];

            for (int sample = 0; sample < NumberOfSamples; sample++)
            {
                var x = intervalStart + intervalLength / NumberOfSamples * sample;
                //var value = noise.OctavePerlin(x, 0.5, 7, 0.5, 1); // Looks a bit like mountains
                //var value = noise.OctavePerlin(x, 0.5, 2, 0.5, 1); // Looks like smooth hills
                var value = 10.0*Math.Pow(noise.OctavePerlin(x, 0.5, 1, 2.0, 2), 8.0); // Mountain peaks
                //var value = noise.perlin(x, 0.5);
                samples[sample] = new Tuple<double, double>(x, value);
            }

            var content = "x,y" + Environment.NewLine + string.Join(Environment.NewLine, samples.Select(s => s.Item1 + "," + s.Item2));
            File.WriteAllText("noise.csv", content);
        }
    }
}
