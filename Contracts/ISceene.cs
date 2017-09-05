namespace Contracts
{
    public interface ISceene
    {
        void Update(float timestep, InputMask inputMask);
        void Activate();
        void Render(double gameTimeMsec);
        void Deactivate();
    }
}