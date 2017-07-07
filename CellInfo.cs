using System;

namespace Sudoku
{
    public abstract class CellInfo
    {
        public const int numbers = 9;
        public const int size = 32;

        public abstract CellInfo Clone();

        public abstract CellInfo HelpClone();

        public abstract void Draw(Drawer drawer, float x, float y, Color color, float alpha = 1f);
    }

    public class Number : CellInfo
    {
        public readonly int Value;
        public readonly bool Starter;

        public Number(int value, bool starter = false)
        {
            if (value < 0 || value >= numbers)
            {
                throw new ArgumentException("Improper number for cell info: " + value);
            }
            this.Value = value;
            this.Starter = starter;
        }

        public override CellInfo Clone()
        {
            return new Number(Value, Starter);
        }

        public override CellInfo HelpClone()
        {
            return new Number(Value, Starter);
        }

        public override bool Equals(object obj)
        {
            Number other = obj as Number;
            return other != null && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override void Draw(Drawer drawer, float x, float y, Color color, float alpha = 1f)
        {
            drawer.Draw(drawer.numbers[Value], x, y, color, alpha);
        }
    }

    public class Notes : CellInfo
    {
        public readonly bool[] Values;
        public readonly Color?[] OverrideColors;
        public const int noteSize = 10;

        public Notes()
        {
            this.Values = new bool[numbers];
            for (int k = 0; k < numbers; k++)
            {
                this.Values[k] = true;
            }
            this.OverrideColors = new Color?[numbers];
        }

        public override CellInfo Clone()
        {
            Notes clone = new Notes();
            for (int k = 0; k < Values.Length; k++)
            {
                clone.Values[k] = Values[k];
            }
            return clone;
        }

        public override CellInfo HelpClone()
        {
            Notes clone = new Notes();
            for (int k = 0; k < Values.Length; k++)
            {
                clone.Values[k] = Values[k];
                clone.OverrideColors[k] = OverrideColors[k];
            }
            return clone;
        }

        public override bool Equals(object obj)
        {
            Notes other = obj as Notes;
            if (other == null)
            {
                return false;
            }
            for (int k = 0; k < Values.Length; k++)
            {
                if (Values[k] != other.Values[k])
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int code = 0;
            for (int k = 0; k < Values.Length; k++)
            {
                code *= 2;
                if (Values[k])
                {
                    code++;
                }
            }
            return code;
        }

        public bool Validate()
        {
            for (int k = 0; k < Values.Length; k++)
            {
                if (Values[k])
                {
                    return true;
                }
            }
            return false;
        }

        public override void Draw(Drawer drawer, float x, float y, Color color, float alpha = 1f)
        {
            drawer.Draw(drawer.noteBackground, x, y, color, alpha);
            for (int k = 0; k < numbers; k++)
            {
                float drawX = x + (noteSize + 1) * (k % 3);
                float drawY = y + (noteSize + 1) * (k / 3);
                Color useColor = OverrideColors[k].HasValue ? OverrideColors[k].Value : color;
                drawer.Draw(Values[k] ? drawer.notes[k] : drawer.emptyNote, drawX, drawY, useColor, alpha);
            }
        }
    }
}
