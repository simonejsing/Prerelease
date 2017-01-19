namespace Contracts
{
    public struct KeyInputs
    {
        public bool Left;
        public bool Right;
        public bool Up;
        public bool Down;
        public bool Select;
    }

    public class InputMask
    {
        public KeyInputs Input = new KeyInputs();

        public InputMask()
        {
            Reset();
        }

        public void Apply(KeyInputs gameInput)
        {
            Input.Left |= gameInput.Left;
            Input.Right |= gameInput.Right;
            Input.Up |= gameInput.Up;
            Input.Down |= gameInput.Down;
            Input.Select |= gameInput.Select;
        }

        public void Reset()
        {
            Input.Left = false;
            Input.Right = false;
            Input.Up = false;
            Input.Down = false;
            Input.Select = false;
        }
    }
}
