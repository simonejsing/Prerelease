namespace Contracts
{
    public interface ISceene
    {
        void Exiting();
        void Update(FrameCounter counter, float timestep);
        void Activate(InputMask uiInput, InputMask[] inputMasks);
        void Render(FrameCounter counter, double gameTimeMsec);
        void Deactivate();
        string[] DiagnosticsString();
    }
}