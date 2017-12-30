using Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main.Input
{
    internal class MonoControllerInput : IMonoInput
    {
        private PlayerIndex index;
        private GamePadState state;
        private InputSet currentInputs = new InputSet();

        public MonoControllerInput(PlayerIndex playerIndex)
        {
            index = playerIndex;
        }

        public InputSet ReadInput()
        {
            state = GamePad.GetState(index);
            currentInputs.Active = state.IsConnected;
            currentInputs.Left = Left();
            currentInputs.Right = Right();
            currentInputs.Up = Up();
            currentInputs.Down = Down();
            currentInputs.Jump = ButtonPressed(state.Buttons.A);
            currentInputs.Attack = ButtonPressed(state.Buttons.X);
            currentInputs.CycleNextEquipment = ButtonPressed(state.Buttons.RightShoulder);
            currentInputs.CyclePreviousEquipment = ButtonPressed(state.Buttons.LeftShoulder);
            currentInputs.Select = ButtonPressed(state.Buttons.Y);
            currentInputs.Restart = ButtonPressed(state.Buttons.Start);
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

        private bool ButtonPressed(ButtonState button)
        {
            return button == ButtonState.Pressed;
        }
    }
}