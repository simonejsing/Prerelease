using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public struct Color
    {
        public float r, g, b;

        public Color(float red, float green, float blue)
        {
            this.r = red;
            this.g = green;
            this.b = blue;
        }

        public static Color Black = new Color(0f, 0f, 0f);
        public static Color White = new Color(1f, 1f, 1f);
        public static Color Red = new Color(1f, 0f, 0f);
        public static Color Green = new Color(0f, 1f, 0f);
        public static Color Blue = new Color(0f, 0f, 1f);
    }
}
