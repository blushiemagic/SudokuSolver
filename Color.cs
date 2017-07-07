using System;
using System.Collections.Generic;

namespace Sudoku
{
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        private static IDictionary<string, Color> colorLookup = new Dictionary<string, Color>();

        public Color(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public Color(int r, int g, int b)
        {
            this.R = (byte)r;
            this.G = (byte)g;
            this.B = (byte)b;
        }

        private Color(string name, byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            colorLookup[name] = this;
        }

        public static Color operator *(Color color, float value)
        {
            byte r = (byte)(color.R * value);
            byte g = (byte)(color.G * value);
            byte b = (byte)(color.B * value);
            return new Color(r, g, b);
        }

        public static Color Lerp(Color color1, Color color2, float value)
        {
            float stick = 1f - value;
            byte r = (byte)(color1.R * stick + color2.R * value);
            byte g = (byte)(color1.G * stick + color2.G * value);
            byte b = (byte)(color1.B * stick + color2.B * value);
            return new Color(r, g, b);
        }

        public static bool GetColor(string name, out Color color)
        {
            return colorLookup.TryGetValue(name, out color);
        }

        public static readonly Color White = new Color("White", 255, 255, 255);

        public static readonly Color Black = new Color("Black", 0, 0, 0);

        public static readonly Color Background = new Color("Background", 0, 250, 154);

        public static readonly Color SelectedCell = new Color("SelectedCell", 135, 206, 250);

        public static readonly Color StartingCell = new Color("StartingCell", 255, 255, 150);

        public static readonly Color Green = new Color("Green", 40, 250, 60);

        public static readonly Color Red = new Color("Red", 255, 30, 30);

        public static readonly Color Blue = new Color("Blue", 90, 100, 250);

        public static readonly Color Orange = new Color("Orange", 250, 160, 60);

        public static readonly Color Purple = new Color("Purple", 250, 100, 250);

        public static readonly Color Cyan = new Color("Cyan", 30, 200, 200);
    }
}
