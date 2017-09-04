namespace Contracts
{
    public interface ISceene
    {
        void ProcessInput(float timestep, InputMask inputMask);
        void Activate();
        void Render(double gameTimeMsec);
        void Deactivate();
    }
}