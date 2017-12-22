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

        public bool KeyPressed(Keys key)
        {
            return pressedKeys.Contains(key);
        }

        private bool Restart()
        {
            return pressedKeys.Contains(Keys.Space);
        }

        private bool Left()
        {
            return pressedKeys.Contains(Keys.Left);
        }

        private bool Right()
        {
            return pressedKeys.Contains(Keys.Right);
        }

        private bool Up()
        {
            return pressedKeys.Contains(Keys.Up);
        }

        private bool Down()
        {
            return pressedKeys.Contains(Keys.Down);
        }

        private bool Attack()
        {
            return pressedKeys.Contains(Keys.E);
        }

        private bool Select()
        {
            return pressedKeys.Contains(Keys.Q);
        }
    }
}
