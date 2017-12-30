using System.Linq;
using Contracts;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main.Input
{
    class MonoKeyboardInput : IMonoInput
    {
        private enum ShiftState { Undefined, Pressed, NotPressed };

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
            currentInputs.Left = KeyDown(Keys.A);
            currentInputs.Right = KeyDown(Keys.D);
            currentInputs.Up = KeyDown(Keys.W);
            currentInputs.Down = KeyDown(Keys.S);
            currentInputs.Jump = KeyDown(Keys.Space);
            currentInputs.Attack = KeyDown(Keys.E);
            currentInputs.CycleNextEquipment = KeyDown(Keys.Q, ShiftState.NotPressed);
            currentInputs.CyclePreviousEquipment = KeyDown(Keys.Q, ShiftState.Pressed);
            currentInputs.Select = KeyDown(Keys.Enter);
            currentInputs.Restart = KeyDown(Keys.Escape);
            return currentInputs;
        }

        private bool KeyDown(Keys key, ShiftState shiftModifier = ShiftState.Undefined)
        {
            switch (shiftModifier)
            {
                case ShiftState.Pressed:
                    return KeyDown(key) && (KeyDown(Keys.LeftShift) || KeyDown(Keys.RightShift));
                case ShiftState.NotPressed:
                    return pressedKeys.Contains(key) && !KeyDown(Keys.LeftShift) && !KeyDown(Keys.RightShift);
                case ShiftState.Undefined:
                default:
                    return pressedKeys.Contains(key);
            }
        }

        private bool KeyDown(Keys key)
        {
            return pressedKeys.Contains(key);
        }
    }
}
