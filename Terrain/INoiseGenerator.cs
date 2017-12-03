namespace Terrain
{
    public interface INoiseGenerator
    {
        double Noise(double x, double y, double amplitude, double frequency, double phase, double damping);
    }
}