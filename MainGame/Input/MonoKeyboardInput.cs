using System.Linq;
using Contracts;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main.Input
{
    class MonoKeyboardInput : IMonoInput
    {
        private Keys[] pressedKeys;
        private InputSet currentInputs = new InputSet();

        public MonoKeyboardInput()
        {
            pressedKeys = new Keys[0];
        }

        public InputSet ReadInput()
        {
            KeyboardState kbState = Keyboard.GetState();
            pressedKeys = kbState.GetPressedKeys();
            currentInputs.Active = true;
            currentInputs.Left = Left();
            currentInputs.Right = Right();
            currentInputs.Up = Up();
            currentInputs.Down = Down();
            currentInputs.Fire = Fire();
            currentInputs.Select = Select();
            currentInputs.Restart = Restart();
            return currentInputs;
        }

        private bool Restart()
        {
            return pressedKeys.Contains(Keys.Space);
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

        public bool Fire()
        {
            return pressedKeys.Contains(Keys.E);
        }

        public bool Select()
        {
            return pressedKeys.Contains(Keys.Enter);
        }
    }
}
