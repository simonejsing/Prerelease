namespace Contracts
{
    public interface ISceene
    {
        void Exiting();
        void Update(float timestep);
        void Activate(InputMask uiInput, InputMask[] inputMasks);
        void Render(double gameTimeMsec);
        void Deactivate();
        string[] DiagnosticsString();
    }
}