using System.Linq;
using Contracts;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main.Input
{
    class MonoKeyboardInput : IMonoInput
    {
        private Keys[] pressedKeys;
        private KeyInputs currentInputs = new KeyInputs();

        public MonoKeyboardInput()
        {
            pressedKeys = new Keys[0];
        }

        public KeyInputs ReadInput()
        {
            KeyboardState kbState = Keyboard.GetState();
            pressedKeys = kbState.GetPressedKeys();
            currentInputs.Left = Left();
            currentInputs.Right = Right();
            currentInputs.Up = Up();
            currentInputs.Down = Down();
            currentInputs.Select = Select();
            return currentInputs;
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
