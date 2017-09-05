namespace Contracts
{
    public struct InputSet
    {
        public bool Left;
        public bool Right;
        public bool Up;
        public bool Down;
        public bool Select;
        public bool Restart;
    }

    public class InputMask
    {
        public InputSet Input = new InputSet();

        public InputMask()
        {
            Reset();
        }

        public void Apply(InputSet gameInput)
        {
            Input.Left |= gameInput.Left;
            Input.Right |= gameInput.Right;
            Input.Up |= gameInput.Up;
            Input.Down |= gameInput.Down;
            Input.Select |= gameInput.Select;
            Input.Restart |= gameInput.Restart;
        }

        public void Reset()
        {
            Input.Left = false;
            Input.Right = false;
            Input.Up = false;
            Input.Down = false;
            Input.Select = false;
            Input.Restart = false;
        }
    }
}
