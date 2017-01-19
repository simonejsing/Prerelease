namespace Contracts
{
    public interface ISceene
    {
        void ProcessInput(double gameTimeMsec, InputMask inputMask);
        void Activate();
        void Render(double gameTimeMsec);
        void Deactivate();
    }
}