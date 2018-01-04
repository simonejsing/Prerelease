using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class PerlinNoise : INoiseGenerator
    {
        private const int GradSize = 2048;
        private double[,] GradX;
        private double[,] GradY;
        private readonly Random random;

        public PerlinNoise(int seed)
        {
            random = new Random(seed);

            // Initialize random gradient map
            InitializeGradientMap();
        }

        private void InitializeGradientMap()
        {
            GradX = new double[GradSize, GradSize];
            GradY = new double[GradSize, GradSize];

            for (int j = 0; j < GradSize; j++)
            {
                for (int i = 0; i < GradSize; i++)
                {
                    var r = random.NextDouble();
                    if (r < 0.25)
                    {
                        GradX[j,i] = 1.0;
                        GradY[j,i] = 1.0;
                    }
                    else if (r < 0.50)
                    {
                        GradX[j,i] = -1.0;
                        GradY[j,i] = 1.0;
                    }
                    else if (r < 0.75)
                    {
                        GradX[j,i] = 1.0;
                        GradY[j,i] = -1.0;
                    }
                    else
                    {
                        GradX[j,i] = -1.0;
                        GradY[j,i] = -1.0;
                    }
                    //GradX[i] = random.NextDouble();
                    //GradY[i] = random.NextDouble();
                }
            }
        }

        public static double fade(double t)
        {
            // 6t^5 - 15t^4 + 10t^3
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static double lerp(double a, double b, double w)
        {
            return (1.0 - w) * a + w * b;
        }

        private static int gradientIndex(int v)
        {
            return Math.Abs(v) % GradSize;
        }

        // Assumes grid size is normalized to 1.
        private double dotGridGradient(int ix, int iy, double x, double y, double damping)
        {
            // Compute gradient at cell x,y
            int gradIndexX = gradientIndex(ix);
            int gradIndexY = gradientIndex(iy);
            double gradx = GradX[gradIndexY, gradIndexX] / damping;
            double grady = GradY[gradIndexY, gradIndexX] / damping;

            // Compute distance vector
            double dx = x - (double) ix;
            double dy = y - (double) iy;

            return dx * gradx + dy * grady;
        }

        // Assumes grid size is normalized to 1.
        private double perlin(double x, double y, double damping = 1.0)
        {
            // Determine grid cell coordinates
            int x0 = (int)Math.Floor(x);
            int y0 = (int)Math.Floor(y);

            // Determine interpolation weights
            // Could also use higher order polynomial/s-curve here
            double sx = x - (double)x0;
            double sy = y - (double)y0;

            double u = fade(sx);
            double v = fade(sy);

            // Interpolate between grid point gradients
            double n0, n1, ix0, ix1, value;
            n0 = dotGridGradient(x0, y0, x, y, damping);
            n1 = dotGridGradient(x0 + 1, y0, x, y, damping);
            ix0 = lerp(n0, n1, u);
            n0 = dotGridGradient(x0, y0 + 1, x, y, damping);
            n1 = dotGridGradient(x0 + 1, y0 + 1, x, y, damping);
            ix1 = lerp(n0, n1, u);
            value = lerp(ix0, ix1, v);

            return (value + 1.0) / 2.0;
            //return value;
        }

        public double Noise(double x, double y, double amplitude, double frequency, double phase, double damping)
        {
            return amplitude * perlin(x * frequency + phase, y * frequency + phase, damping);
        }
    }
}
