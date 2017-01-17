using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main
{
    class MonoKeyboardInput : IMonoInput
    {
        private Keys[] pressedKeys;

        public MonoKeyboardInput()
        {
            pressedKeys = new Keys[0];
        }

        public void Update()
        {
            KeyboardState kbState = Keyboard.GetState();
            pressedKeys = kbState.GetPressedKeys();

        }

        public bool Left()
        {
            return pressedKeys.Contains(Keys.A);
        }

        public bool Right()
        {
            return pressedKeys.Contains(Keys.D);
        }

        public bool Up()
        {
            return pressedKeys.Contains(Keys.W);
        }

        public bool Down()
        {
            return pressedKeys.Contains(Keys.S);
        }

        public bool Select()
        {
            return pressedKeys.Contains(Keys.Enter);
        }
    }
}
