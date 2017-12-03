namespace Contracts
{
    public interface ISceene
    {
        void Update(float timestep);
        void Activate(InputMask uiInput, InputMask[] inputMasks);
        void Render(double gameTimeMsec);
        void Deactivate();
    }
}