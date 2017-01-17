using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Prerelease.Main
{
    internal class MonoControllerInput : IMonoInput
    {
        private PlayerIndex index;
        private GamePadState state;

        public MonoControllerInput(PlayerIndex playerIndex)
        {
            index = playerIndex;
        }

        public void Update()
        {
            state = GamePad.GetState(index);
        }

        public bool Left()
        {
            return state.ThumbSticks.Left.X < 0.0;
        }

        public bool Right()
        {
            return state.ThumbSticks.Left.X > 0.0;
        }

        public bool Up()
        {
            return state.ThumbSticks.Left.Y > 0.0;
        }

        public bool Down()
        {
            return state.ThumbSticks.Left.Y < 0.0;
        }

        public bool Select()
        {
            return state.Buttons.A == ButtonState.Pressed;
        }
    }
}