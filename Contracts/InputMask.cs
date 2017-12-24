namespace Contracts
{
    public struct InputSet
    {
        public bool Active;
        public bool Left;
        public bool Right;
        public bool Up;
        public bool Down;
        public bool Attack;
        public bool Jump;
        public bool Select;
        public bool Restart;

        public bool Moving => Active && (Left || Right || Up || Down || Jump);
    }

    public class InputMask
    {
        public bool Bound { get; set; }

        public InputSet Input = new InputSet();

        public string PlayerBinding { get; }

        public InputMask(string playerBinding)
        {
            Bound = false;
            PlayerBinding = playerBinding;
            Reset();
        }

        public void Apply(InputSet gameInput)
        {
            Input.Active |= gameInput.Active;
            Input.Left |= gameInput.Left;
            Input.Right |= gameInput.Right;
            Input.Up |= gameInput.Up;
            Input.Down |= gameInput.Down;
            Input.Attack |= gameInput.Attack;
            Input.Jump |= gameInput.Jump;
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
            Input.Attack = false;
            Input.Jump = false;
            Input.Select = false;
            Input.Restart = false;
        }
    }
}
