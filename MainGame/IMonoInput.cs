namespace Prerelease.Main
{
    internal interface IMonoInput
    {
        void Update();
        bool Left();
        bool Right();
        bool Up();
        bool Down();
        bool Select();
    }
}