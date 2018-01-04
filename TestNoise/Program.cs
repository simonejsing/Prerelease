using Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using Color = System.Drawing.Color;

namespace TestNoise
{
    class Program
    {
        static void Main(string[] args)
        {
            //RenderTerrain();

            //RenderDensityMap();

            EvaluateTerrain();

            //SaveNoise();
        }

        private static void SaveNoise()
        {
            const int maxHeight = 100;
            const int maxDepth = 100;
            const int seaLevel = 80;
            var generator = new Generator(maxDepth, maxHeight, seaLevel, 0);

            //var samples = IronOreLevels(generator);
            var samples = SeabedTerrain();
            //var samples = BedrockTerrain();
            //var samples = SampleNoise(generator.DirtLevel);

            var content = "x,y" + Environment.NewLine +
                          string.Join(Environment.NewLine, samples.Select(s => s.Item1 + "," + s.Item2));
            File.WriteAllText("noise.csv", content);
        }

        private static void RenderTerrain()
        {
            const int maxHeight = 100;
            const int maxDepth = 100;
            const int seaLevel = 80;
            var generator = new TerrainInverter(new Generator(maxDepth, maxHeight, seaLevel, 0));

            const double scale = 10.0;
            const int sizeX = 1600;
            //const int sizeY = (int)(2 * maxHeight / scale);
            const int sizeY = (int) (maxDepth + maxHeight);
            var bitmap = new Bitmap(sizeX, sizeY);
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    var coord = new Coordinate((int)(x * scale), y - sizeY / 2);
                    //var py = y; // * scale;
                    var c = Color.Black;
                    var block = generator[coord, new Plane(0)];
                    switch (block.Type)
                    {
                        case TerrainType.Bedrock:
                            c = Color.Gray;
                            break;
                        case TerrainType.Rock:
                            c = Color.DarkGray;
                            //var red = Math.Max(0, Math.Min(255, (int)(block.OreDensity * 255)));
                            //c = Color.FromArgb(red, 0, 0);
                            break;
                        case TerrainType.Dirt:
                            c = Color.SandyBrown;
                            break;
                        case TerrainType.Sea:
                            c = Color.Aqua;
                            break;
                    }
                    bitmap.SetPixel(x, y, c);
                }
            }

            bitmap.Save("map.bmp");
        }

        private static void RenderDensityMap()
        {
            const int maxHeight = 100;
            const int maxDepth = 100;
            const int seaLevel = 80;
            var generator = new Generator(maxDepth, maxHeight, seaLevel, 0);
            //var generator = new OctaveNoise(new PerlinNoise(0), 4, 0.1);
            //var generator = new PerlinNoise(0);

            const int sizeX = 1000;
            const int sizeY = maxHeight + maxDepth;
            const int scale = 1;

            var bitmap = new Bitmap(sizeX, sizeY);
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    var c = Color.Black;
                    //var value = Perlin.perlin(x + 0.5, y + 0.5, 0);
                    //var value = generator.Noise(x + 0.5, y + 0.5, 1.0, 1f / 100f, 0f, 1f);
                    //var value = generator.IronOreDensity(x, y, 0);
                    var value = generator.GoldOreDensity(x * scale, y - maxDepth, 0);
                    //var value = generator.DiamondOreDensity(x * scale, y * scale, 0);
                    var colorTone = Math.Max(0, Math.Min(255, (int)(value * 255)));
                    c = Color.FromArgb(colorTone, colorTone, colorTone);
                    bitmap.SetPixel(x, y, c);
                }
            }

            bitmap.Save("density.bmp");
        }

        private static void EvaluateTerrain()
        {
            const int maxHeight = 100;
            const int maxDepth = 100;
            const int seaLevel = 80;
            var generator = new Generator(maxDepth, maxHeight, seaLevel, 1050);

            var total = 0.0;
            var numberRock = 0.0;
            var numberIron = 0.0;
            var numberGold = 0.0;
            var numberDiamond = 0.0;

            // Evaluate statistics for the generated terrain
            const int maxX = 10000;
            var plane = new Plane(0);
            for(int x = 0; x < maxX; x++)
            {
                for (int y = -maxDepth; y <= 0; y++)
                {
                    var block = generator[new Coordinate(x, y), plane];
                    total++;

                    // If dirt or water then we can skip the rest of the slice
                    if (block.Type == TerrainType.Dirt || block.Type == TerrainType.Sea)
                    {
                        total += 0 - y;
                        break;
                    }

                    if (block.Type == TerrainType.Rock)
                        numberRock++;

                    if (block.Ore == OreType.Iron)
                        numberIron++;

                    if (block.Ore == OreType.Gold)
                        numberGold++;

                    if (block.Ore == OreType.Diamond)
                        numberDiamond++;
                }
            }

            // Print statistics
            Console.WriteLine("Total blocks evaluated: {0}", total);
            Console.WriteLine("Rock %: {0}", numberRock / total);
            Console.WriteLine("Densities:");
            Console.WriteLine("Iron: {0} ({1})", numberIron / numberRock, numberIron);
            Console.WriteLine("Gold: {0} ({1})", numberGold / numberRock, numberGold);
            Console.WriteLine("Diamond: {0} ({1})", numberDiamond / numberRock, numberDiamond);
        }

        private static Tuple<double, double>[] SampleNoise(Func<double, double, double> noiseFunc)
        {
            // Sample area
            const double intervalLength = 1000.0;
            const int numberOfSamples = 500;

            var samples = new Tuple<double, double>[numberOfSamples];
            for (int sample = 0; sample < numberOfSamples; sample++)
            {
                var x = intervalLength / numberOfSamples * sample;
                var value = noiseFunc(x, 0.5);
                samples[sample] = new Tuple<double, double>(x, value);
            }
            return samples;
        }

        private static Tuple<double, double>[] SeabedTerrain()
        {
            // Noise parameters
            const double frequency = 1.0 / 4096.0;
            const double amplitude = 1.0;
            const double exponent = 7.0;
            const int octaves = 4;
            const double persistence = 0.08;
            const double damping = 1.0;

            // Sample area
            const double intervalStart = 10.0; // Phase shift
            const double intervalLength = 20000.0;
            const int numberOfSamples = 500;

            var generator = new OctaveNoise(new PerlinNoise(0), octaves, persistence);
            var samples = new Tuple<double, double>[numberOfSamples];
            for (int sample = 0; sample < numberOfSamples; sample++)
            {
                var x = intervalLength / numberOfSamples * sample;
/*                var value =
                    100.0 * Math.Pow(generator.Noise(x, 0.5, amplitude, frequency, intervalStart, damping), exponent) /
                    Math.Pow(amplitude, exponent);*/
                var value =
                    10.0 * Math.Pow(generator.Noise(x, 0.5, amplitude, frequency, intervalStart, damping), exponent) - 0.5;
                value = (1.0 - Softmax(10.0*value));
                samples[sample] = new Tuple<double, double>(x, value);
            }
            return samples;
        }

        private static double Softmax(double x)
        {
            return 1.0 / (1 + Math.Exp(-x));
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
