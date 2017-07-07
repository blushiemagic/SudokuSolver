using System;
using System.Collections.Generic;
using System.Threading;

namespace Sudoku
{
    public partial class Board
    {
        public bool settingUp = true;
        public bool Paused => !settingUp && solution == null;
        public Cell hoverCell;
        public Cell selectedCell;
        private bool doSetup = false;
        private bool doClean = false;
        private Control[] numberKeys = new Control[]
        {
            Control.D1,
            Control.D2,
            Control.D3,
            Control.D4,
            Control.D5,
            Control.D6,
            Control.D7,
            Control.D8,
            Control.D9
        };
        public List<Button> buttons = new List<Button>();

        private void InitializeButtons()
        {
            for (int k = 0; k < 10; k++)
            {
                Button.ButtonType type = Button.ButtonType.Fill_1 + k;
                buttons.Add(new Button(type, Main.Width / 2 + (int)(44 * (k - 4.5f)), Main.Height - 180));
            }
            buttons.Add(new Button(Button.ButtonType.Start, Main.Width / 2, Main.Height - 100));
            buttons.Add(new Button(Button.ButtonType.Save, Main.Width / 2 - 80, Main.Height - 100));
            buttons.Add(new Button(Button.ButtonType.Load, Main.Width / 2 + 80, Main.Height - 100));
        }

        public void Update()
        {
            if (Paused)
            {
                solveTime++;
                return;
            }
            hoverCell = null;
            transition += 0.05f;
            if (transition > 1f)
            {
                transition = 1f;
            }
            textboxTransition += 0.1f;
            if (textboxTransition > 1f)
            {
                textboxTransition = 1f;
            }
            foreach (Cell cell in board)
            {
                cell.Update();
            }
            if (Main.input.MouseClick)
            {
                bool flag = true;
                foreach (Button button in buttons)
                {
                    if (button.IsMouseOver())
                    {
                        ClickButton(button.type);
                        flag = false;
                    }
                }
                if (settingUp && flag)
                {
                    selectedCell = hoverCell;
                }
            }
            if (selectedCell != null)
            {
                if (Main.input.KeyType(Control.Back))
                {
                    selectedCell.info = null;
                }
                for (int k = 0; k < numberKeys.Length; k++)
                {
                    if (Main.input.KeyType(numberKeys[k]))
                    {
                        selectedCell.info = new Number(k, true);
                    }
                }
            }
            if (doSetup)
            {
                settingUp = false;
                selectedCell = null;
                buttons.Clear();
                buttons.Add(new Button(Button.ButtonType.Reset, Main.Width / 2 - 120, Main.Height - 180));
                buttons.Add(new Button(Button.ButtonType.Undo, Main.Width / 2 - 60, Main.Height - 180));
                buttons.Add(new Button(Button.ButtonType.Next, Main.Width / 2 + 60, Main.Height - 180));
                buttons.Add(new Button(Button.ButtonType.Finish, Main.Width / 2 + 120, Main.Height - 180));
                buttons.Add(new Button(Button.ButtonType.Clean, Main.Width / 2, Main.Height - 180));
                buttons.Add(new Button(Button.ButtonType.UndoHelp, 40, Main.Height - 80));
                buttons.Add(new Button(Button.ButtonType.NextHelp, Main.Width - 40, Main.Height - 80));
                StartSolution();
                ThreadPool.QueueUserWorkItem(state => solution = Solve());
                doSetup = false;
            }
            else if (doClean)
            {
                settingUp = true;
                buttons.Clear();
                InitializeButtons();
                DeletePuzzle();
                doClean = false;
            }
        }

        private void ClickButton(Button.ButtonType type)
        {
            if (type == Button.ButtonType.Start)
            {
                if (ValidatePuzzle())
                {
                    doSetup = true;
                }
            }
            else if (type == Button.ButtonType.Reset)
            {
                Reset();
            }
            else if (type == Button.ButtonType.Undo)
            {
                Undo();
            }
            else if (type == Button.ButtonType.Next)
            {
                Next();
            }
            else if (type == Button.ButtonType.Finish)
            {
                Finish();
            }
            else if (type == Button.ButtonType.Clean)
            {
                doClean = true;
            }
            else if (type == Button.ButtonType.Save)
            {
                Save();
            }
            else if (type == Button.ButtonType.Load)
            {
                Load();
            }
            else if (type >= Button.ButtonType.Fill_1 && type <= Button.ButtonType.Fill_9)
            {
                if (selectedCell != null)
                {
                    selectedCell.info = new Number(type - Button.ButtonType.Fill_1, true);
                }
            }
            else if (type == Button.ButtonType.Backspace)
            {
                if (selectedCell != null)
                {
                    selectedCell.info = null;
                }
            }
            else if (type == Button.ButtonType.UndoHelp)
            {
                UndoHelp();
            }
            else if (type == Button.ButtonType.NextHelp)
            {
                NextHelp();
            }
        }

        public void DrawButtons(Drawer drawer)
        {
            foreach (Button button in buttons)
            {
                button.Draw(drawer);
            }
        }
    }

    public class Button
    {
        public enum ButtonType
        {
            Start,
            Reset,
            Undo,
            Next,
            Finish,
            Clean,
            Save,
            Load,
            Fill_1,
            Fill_2,
            Fill_3,
            Fill_4,
            Fill_5,
            Fill_6,
            Fill_7,
            Fill_8,
            Fill_9,
            Backspace,
            NextHelp,
            UndoHelp
        }

        public readonly ButtonType type;
        public int X;
        public int Y;
        public const int Width = 40;
        public const int Height = 40;

        public Button(ButtonType type, int x, int y)
        {
            this.type = type;
            this.X = x - Width / 2;
            this.Y = y - Height / 2;
        }

        public bool IsMouseOver()
        {
            Point mousePos = Main.input.MousePos;
            return mousePos.X > X && mousePos.X < X + Width && mousePos.Y > Y && mousePos.Y < Y + Height;
        }

        public void Draw(Drawer drawer)
        {
            Color color = Color.White;
            if (IsMouseOver())
            {
                color *= 0.8f;
            }
            drawer.Draw(drawer.buttons[(int)type], X, Y, color);
        }
    }
}
