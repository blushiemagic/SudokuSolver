using System;
using System.Collections.Generic;

namespace Sudoku
{
    public abstract class Drawer
    {
        public object cell;
        public object box;
        public object[] numbers;
        public object[] notes;
        public object emptyNote;
        public object noteBackground;
        public object[] buttons;
        protected object textBoxTopLeft;
        protected object textBoxTopRight;
        protected object textBoxBottomRight;
        protected object textBoxBottomLeft;
        protected object textBoxTopBottom;
        protected object textBoxLeftRight;
        protected object textBoxCenter;

        public abstract object LoadImage(string path);

        internal void LoadContent()
        {
            cell = LoadImage("Cell");
            box = LoadImage("Box");
            numbers = new object[CellInfo.numbers];
            notes = new object[CellInfo.numbers];
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                numbers[k] = LoadImage("Numbers/Number_" + (k + 1));
                notes[k] = LoadImage("Numbers/Note_" + (k + 1));
            }
            emptyNote = LoadImage("Numbers/Note_Empty");
            noteBackground = LoadImage("Numbers/Note_Back");
            string[] buttonNames = Enum.GetNames(typeof(Button.ButtonType));
            buttons = new object[buttonNames.Length];
            for (int k = 0; k < buttonNames.Length; k++)
            {
                buttons[k] = LoadImage("Buttons/" + buttonNames[k]);
            }
            textBoxTopLeft = LoadImage("TextBox_TopLeft");
            textBoxTopRight = LoadImage("TextBox_TopRight");
            textBoxBottomRight = LoadImage("TextBox_BottomRight");
            textBoxBottomLeft = LoadImage("TextBox_BottomLeft");
            textBoxTopBottom = LoadImage("TextBox_TopBottom");
            textBoxLeftRight = LoadImage("TextBox_LeftRight");
            textBoxCenter = LoadImage("TextBox_Center");
            LoadExtraContent();
        }

        public virtual void LoadExtraContent()
        {
        }

        public abstract void Clear(Color color);

        public abstract void BeginDraw();

        public abstract void EndDraw();

        public abstract void Draw(object image, float x, float y, Color color, float alpha = 1f, float scale = 1f);

        public void Draw(object image, float x, float y, float scale = 1f)
        {
            Draw(image, x, y, Color.White, 1f, scale);
        }

        public abstract void DrawTextBox(float x, float y, float width, float height, float alpha = 1f);

        public abstract void DrawText(float x, float y, string text, Color color, float alpha = 1f, float scale = 1f);

        public abstract void DrawBigText(float x, float y, string text, Color color, float alpha = 1f);

        public void DrawText(float x, float y, string text)
        {
            DrawText(x, y, text, Color.Black);
        }

        public void DrawBigText(float x, float y, string text)
        {
            DrawBigText(x, y, text, Color.Black);
        }

        public abstract float MeasureTextWidth(string text);

        public abstract float MeasureTextHeight(string text);

        public abstract float MeasureBigTextWidth(string text);

        public abstract float MeasureBigTextHeight(string text);

        public void DrawTextBox(float x, float y, float width, float height, TextboxInfo textboxInfo, float alpha = 1f)
        {
            string text = textboxInfo.Text;
            string[] args = textboxInfo.Arguments;
            DrawTextBox(x, y, width, height, alpha);
            List<string> words = new List<string>();
            List<bool> spaces = new List<bool>();
            int k = 0;
            while (k < text.Length)
            {
                if (text[k] == ' ')
                {
                    k++;
                }
                else if (text[k] == '[')
                {
                    int l;
                    for (l = k + 1; l < text.Length; l++)
                    {
                        if (text[l] == ']')
                        {
                            break;
                        }
                    }
                    if (l < text.Length)
                    {
                        words.Add(text.Substring(k, l + 1 - k));
                        spaces.Add(l + 1 < text.Length && text[l + 1] == ' ');
                        k = l + 1;
                    }
                }
                else
                {
                    int l;
                    bool addSpace = true;
                    for (l = k + 1; l < text.Length; l++)
                    {
                        if (text[l] == ' ' || text[l] == '[')
                        {
                            if (text[l] == '[')
                            {
                                addSpace = false;
                            }
                            break;
                        }
                    }
                    spaces.Add(addSpace);
                    words.Add(text.Substring(k, l - k));
                    k = l;
                    if (addSpace)
                    {
                        k++;
                    }
                }
            }
            Color color = Color.Black;
            float charX = 0f;
            float charY = 0f;
            float lineHeight = 0f;
            for (k = 0; k < words.Count; k++)
            {
                string word = words[k];
                string realWord = word;
                bool drawWord = false;
                if (word[0] == '[' && word[word.Length - 1] == ']' && word.Length > 4)
                {
                    if (word[2] != ':')
                    {
                        drawWord = true;
                    }
                    else if (word[1] == 'c')
                    {
                        string name = word.Substring(3, word.Length - 4);
                        Color tryColor;
                        if (Color.GetColor(name, out tryColor))
                        {
                            color = tryColor;
                        }
                        else
                        {
                            drawWord = true;
                        }
                    }
                    else if (word[1] == 'a')
                    {
                        string number = word.Substring(3, word.Length - 4);
                        int arg;
                        if (int.TryParse(number, out arg))
                        {
                            realWord = args[arg];
                        }
                        drawWord = true;
                    }
                    else
                    {
                        drawWord = true;
                    }
                }
                else
                {
                    drawWord = true;
                }
                if (drawWord)
                {
                    float wordWidth = MeasureTextWidth(realWord);
                    float wordHeight = MeasureTextHeight(realWord);
                    if (charX > 0f && charX + wordWidth > width - 8f)
                    {
                        charX = 0f;
                        charY += lineHeight + 4f;
                        lineHeight = 0f;
                    }
                    if (wordHeight > lineHeight)
                    {
                        lineHeight = wordHeight;
                    }
                    DrawText((int)(x + 4f + charX), (int)(y + 4f + charY), realWord, color);
                    charX += wordWidth;
                }
                if (spaces[k])
                {
                    charX += 8f;
                }
            }
        }
    }
}
