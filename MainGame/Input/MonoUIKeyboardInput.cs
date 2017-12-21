using System.Linq;
using Contracts;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main.Input
{
    class MonoUIKeyboardInput : IMonoInput
    {
        private Keys[] pressedKeys;
        private InputSet currentInputs = new InputSet();

        public MonoUIKeyboardInput()
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
            currentInputs.Attack = Attack();
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
            return pressedKeys.Contains(Keys.Left);
        }

        public bool Right()
        {
            return pressedKeys.Contains(Keys.Right);
        }

        public bool Up()
        {
            return pressedKeys.Contains(Keys.Up);
        }

        public bool Down()
        {
            return pressedKeys.Contains(Keys.Down);
        }

        public bool Attack()
        {
            return pressedKeys.Contains(Keys.E);
        }

        public bool Select()
        {
            return pressedKeys.Contains(Keys.Q);
        }
    }
}
