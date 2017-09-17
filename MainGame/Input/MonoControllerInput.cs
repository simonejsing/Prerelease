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
            currentInputs.Fire = Fire();
            currentInputs.Select = Select();
            currentInputs.Restart = Restart();
            return currentInputs;
        }

        private bool Restart()
        {
            return state.Buttons.Start == ButtonState.Pressed;
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
            //return state.ThumbSticks.Left.Y > 0.0;
            return state.Buttons.A == ButtonState.Pressed;
        }

        private bool Down()
        {
            return state.ThumbSticks.Left.Y < 0.0;
        }

        private bool Fire()
        {
            return state.Buttons.X == ButtonState.Pressed;
        }

        private bool Select()
        {
            return state.Buttons.Y == ButtonState.Pressed;
        }
    }
}