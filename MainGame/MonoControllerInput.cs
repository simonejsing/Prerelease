using Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main
{
    internal class MonoControllerInput : IMonoInput
    {
        private PlayerIndex index;
        private GamePadState state;
        private KeyInputs currentInputs = new KeyInputs();

        public MonoControllerInput(PlayerIndex playerIndex)
        {
            index = playerIndex;
        }

        public KeyInputs ReadInput()
        {
            state = GamePad.GetState(index);
            currentInputs.Left = Left();
            currentInputs.Right = Right();
            currentInputs.Up = Up();
            currentInputs.Down = Down();
            currentInputs.Select = Select();
            return currentInputs;
        }

        private bool Left()
        {
            return state.ThumbSticks.Left.X < 0.0;
        }

        private bool Right()
        {
            return state.ThumbSticks.Left.X > 0.0;
        }

        private bool Up()
        {
            return state.ThumbSticks.Left.Y > 0.0;
        }

        private bool Down()
        {
            return state.ThumbSticks.Left.Y < 0.0;
        }

        private bool Select()
        {
            return state.Buttons.A == ButtonState.Pressed;
        }
    }
}