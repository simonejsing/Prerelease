using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class SineNoise : INoiseGenerator
    {
        public double Noise(double x, double y, double amplitude, double frequency, double phase, double damping)
        {
            return amplitude * Math.Sin(2 * Math.PI * frequency * x / 600.0 + phase);
        }
    }
}
