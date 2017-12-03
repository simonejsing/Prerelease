using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public class OctaveNoise : INoiseGenerator
    {
        private INoiseGenerator innerGenerator;
        private int octaves;
        private double persistence;

        public OctaveNoise(INoiseGenerator innerGenerator, int octaves, double persistence)
        {
            this.innerGenerator = innerGenerator;
            this.octaves = octaves;
            this.persistence = persistence;
        }

        public double Noise(double x, double y, double amplitude, double frequency, double phase, double damping)
        {
            double total = 0;
            double octave_amplitude = 1;
            double maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < octaves; i++)
            {
                total += innerGenerator.Noise(x, y, octave_amplitude, frequency, phase, damping);

                maxValue += octave_amplitude;

                octave_amplitude *= persistence;
                frequency *= 2;
            }

            return amplitude * total / maxValue;
        }
    }
}
