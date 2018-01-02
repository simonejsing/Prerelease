using System;

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

        public bool CycleNextEquipment;
        public bool CyclePreviousEquipment;

        public bool Moving => Active && (Left || Right || Up || Down || Jump);
    }

    public class InputMask
    {
        public bool Bound { get; set; }

        public InputSet PreviousInput = new InputSet();
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
            Input.CycleNextEquipment |= gameInput.CycleNextEquipment;
            Input.CyclePreviousEquipment |= gameInput.CyclePreviousEquipment;
            Input.Select |= gameInput.Select;
            Input.Restart |= gameInput.Restart;
        }

        public bool InputToggled(Func<InputSet, bool> predicat)
        {
            var previousInputState = predicat(PreviousInput);
            var inputState = predicat(Input);
            return previousInputState != inputState;
        }

        public bool InputToggled(Func<InputSet, bool> predicat, bool desiredState)
        {
            var previousInputState = predicat(PreviousInput);
            var inputState = predicat(Input);
            return inputState == desiredState && previousInputState != inputState;
        }

        public void Reset()
        {
            PreviousInput = Input;

            Input.Active = false;
            Input.Left = false;
            Input.Right = false;
            Input.Up = false;
            Input.Down = false;
            Input.Attack = false;
            Input.Jump = false;
            Input.CycleNextEquipment = false;
            Input.CyclePreviousEquipment = false;
            Input.Select = false;
            Input.Restart = false;
        }
    }
}
