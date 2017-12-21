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
            currentInputs.Jump = Jump();
            currentInputs.Attack = Attack();
            currentInputs.Select = Select();
            currentInputs.Restart = Restart();
            return currentInputs;
        }

        private bool Restart()
        {
            return pressedKeys.Contains(Keys.Escape);
        }

        private bool Left()
        {
            return pressedKeys.Contains(Keys.A);
        }

        private bool Right()
        {
            return pressedKeys.Contains(Keys.D);
        }

        private bool Up()
        {
            return pressedKeys.Contains(Keys.W);
        }

        private bool Down()
        {
            return pressedKeys.Contains(Keys.S);
        }

        private bool Jump()
        {
            return pressedKeys.Contains(Keys.Space);
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
