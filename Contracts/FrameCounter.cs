using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public class FrameCounter
    {
        public long FrameNumber { get; private set; } = -1;
        public bool TenthFrame { get; private set; } = true;
        public bool HundredFrame { get; private set; } = true;

        public void Inc()
        {
            FrameNumber++;
            TenthFrame = FrameNumber % 10 == 0;
            HundredFrame = FrameNumber % 100 == 0;
        }
    }
}
