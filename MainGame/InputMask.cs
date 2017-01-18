using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prerelease.Main
{
    struct KeyInputs
    {
        public bool Up, Down, Select;
    }

    internal class InputMask
    {
        public KeyInputs Input = new KeyInputs();

        public InputMask()
        {
            Reset();
        }

        public void Apply(IMonoInput gameInput)
        {
            Input.Up |= gameInput.Up();
            Input.Down |= gameInput.Down();
            Input.Select |= gameInput.Select();
        }

        public void Reset()
        {
            Input.Down = false;
            Input.Up = false;
            Input.Select = false;
        }
    }
}
