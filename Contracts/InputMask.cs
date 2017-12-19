namespace Contracts
{
    public struct InputSet
    {
        public bool Active;
        public bool Left;
        public bool Right;
        public bool Up;
        public bool Down;
        public bool Fire;
        public bool Select;
        public bool Restart;
    }

    public class InputMask
    {
        public InputSet Input = new InputSet();

        public bool Moving => Input.Active && (Input.Left || Input.Right || Input.Up || Input.Down);

        public InputMask()
        {
            Reset();
        }

        public void Apply(InputSet gameInput)
        {
            Input.Active |= gameInput.Active;
            Input.Left |= gameInput.Left;
            Input.Right |= gameInput.Right;
            Input.Up |= gameInput.Up;
            Input.Down |= gameInput.Down;
            Input.Fire |= gameInput.Fire;
            Input.Select |= gameInput.Select;
            Input.Restart |= gameInput.Restart;
        }

        public void Reset()
        {
            Input.Active = false;
            Input.Left = false;
            Input.Right = false;
            Input.Up = false;
            Input.Down = false;
            Input.Fire = false;
            Input.Select = false;
            Input.Restart = false;
        }
    }
}
