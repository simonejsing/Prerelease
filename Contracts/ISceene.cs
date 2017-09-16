namespace Contracts
{
    public interface ISceene
    {
        void Update(float timestep, InputMask[] inputMasks);
        void Activate();
        void Render(double gameTimeMsec);
        void Deactivate();
    }
}