﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public struct Color
    {
        public float r, g, b, a;

        public Color(float red, float green, float blue, float alpha = 0f)
        {
            this.r = red;
            this.g = green;
            this.b = blue;
            this.a = alpha;
        }

        public static Color Black = new Color(0f, 0f, 0f, 1f);
        public static Color White = new Color(1f, 1f, 1f, 1f);
        public static Color Red = new Color(1f, 0f, 0f, 1f);
        public static Color Green = new Color(0f, 1f, 0f, 1f);
        public static Color Blue = new Color(0f, 0f, 1f, 1f);
        public static Color Yellow = new Color(1f, 1f, 0f, 1f);
        public static Color LightGray = new Color(0.8f, 0.8f, 0.8f, 1f);
        public static Color Gray = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static Color DarkGray = new Color(0.1f, 0.1f, 0.1f, 1f);
    }
}
