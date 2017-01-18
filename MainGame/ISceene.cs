using Microsoft.Xna.Framework;

namespace Prerelease.Main
{
    internal interface ISceene
    {
        void ProcessInput(GameTime gameTime, InputMask inputMask);
        void Activate();
        void Render(GameTime gameTime);
        void Deactivate();
    }
}