using System;
using Microsoft.Xna.Framework.Input;

namespace Sudoku
{
    public class ComputerInput : Input
    {
        public override Point MousePos
        {
            get
            {
                Microsoft.Xna.Framework.Point point = Mouse.GetState().Position;
                return new Point(point.X, point.Y);
            }
        }
        private static readonly Keys[] keys = new Keys[(int)Control.Count];
        private bool[] keyType = new bool[(int)Control.Count];
        private bool[] keyPress = new bool[(int)Control.Count];
        private bool[] keyRelease = new bool[(int)Control.Count];

        static ComputerInput()
        {
            keys[(int)Control.D1] = Keys.D1;
            keys[(int)Control.D2] = Keys.D2;
            keys[(int)Control.D3] = Keys.D3;
            keys[(int)Control.D4] = Keys.D4;
            keys[(int)Control.D5] = Keys.D5;
            keys[(int)Control.D6] = Keys.D6;
            keys[(int)Control.D7] = Keys.D7;
            keys[(int)Control.D8] = Keys.D8;
            keys[(int)Control.D9] = Keys.D9;
            keys[(int)Control.Back] = Keys.Back;
        }

        public override void Update()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                MouseClick = !MousePress;
                MousePress = true;
                MouseRelease = false;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                MouseClick = false;
                MouseRelease = MousePress;
                MousePress = false;
            }
            KeyboardState keyboard = Keyboard.GetState();
            for (int k = 0; k < (int)Control.Count; k++)
            {
                if (keyboard.IsKeyDown(keys[k]))
                {
                    keyType[k] = !keyPress[k];
                    keyPress[k] = true;
                    keyRelease[k] = false;
                }
                else if (keyboard.IsKeyUp(keys[k]))
                {
                    keyType[k] = false;
                    keyRelease[k] = keyPress[k];
                    keyPress[k] = false;
                }
            }
        }

        public override bool KeyType(Control control)
        {
            return keyType[(int)control];
        }

        public override bool KeyPress(Control control)
        {
            return keyPress[(int)control];
        }
        
        public override bool KeyRelease(Control control)
        {
            return keyPress[(int)control];
        }
    }
}
