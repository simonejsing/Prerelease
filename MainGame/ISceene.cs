namespace Prerelease.Main
{
    internal interface ISceene
    {
        void ProcessInput(IMonoInput inputs);
        void Activate();
        void Render();
        void Deactivate();
    }
}