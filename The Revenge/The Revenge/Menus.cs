using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GameSystem
{

    //WARNING, COMPLICATED. ASK KYLE FOR HELP AS ALL OF THESE ARE EASY TO SCREW UP AND EACH HAVE THEIR OWN SETUP

    //extend this if your state needs holding down buttons to tick through one at a time
    //if you want to use this it is important to call the startMenu() and endMenu() functions!
    abstract class MenuState : GameState
    {
        protected bool keyA = true, keyB = true, keyUp = true, keyDown = true, keyStart = true, keySelect = true, keyLeft = true, keyRight = true;
        protected bool pKeyA = false, pKeyB = false, pKeyUp = false, pKeyDown = false, pKeyStart = false, pKeySelect = false, pKeyLeft = false, pKeyRight = false;
        public MenuState(Player p) : base(p)
        {
            this.p = p;
        }

        //call this at the beginning of a run() method
        public void startMenu()
        {
            pKeyA = p.getKey("A");
            pKeyB = p.getKey("B");
            pKeyUp = p.getKey("Up");
            pKeyDown = p.getKey("Down");
            pKeyLeft = p.getKey("Left");
            pKeyRight = p.getKey("Right");
            pKeyStart = p.getKey("Start");
            pKeySelect = p.getKey("Select");
        }

        //call this at the end of the run() method
        public void endMenu()
        {
            keyA = pKeyA;
            keyB = pKeyB;
            keyUp = pKeyUp;
            keyDown = pKeyDown;
            keyLeft = pKeyLeft;
            keyRight = pKeyRight;
            keyStart = pKeyStart;
            keySelect = pKeySelect;
        }
    }

    class OptionPane : MenuState
    {
        int option = 0;
        string[] options;
        int width = 1, height, yOffset;
        GameState caller;
        int flag;
        string message;
        TextBox textBox = null;

        public OptionPane(GameState caller, string[] options, int flag, string message, int yOffset, Player p) : base(p)
        {
            this.caller = caller;
            this.options = options;
            this.flag = flag;
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Length > width)
                    width = options[i].Length;
            }
            height = options.Length;
            this.yOffset = yOffset;
            this.message = message;
            if (message != "")
            {
                textBox = new TextBox(caller, p, message);
                if (this.yOffset < 5)
                    this.yOffset += 5;
            }
        }

        public override void draw(Graphics g, Player p)
        {
            if (textBox != null) textBox.draw(g, p);
            int y = 16 - height - yOffset;
            Sprites.drawBox(g, 18 - width, y, width + 2, height + 2);
            for (int i = 0; i < options.Length; i++)
            {
                Sprites.drawString(g, options[i], 19 - width, y + i + 1);
            }
            g.DrawRectangle(new Pen(Color.Black), 8 * (19 - width) - 1, 8 * (y + option + 1) - 1, width * 8 + 2, 10);
        }

        public override void run(Player p)
        {
            startMenu();
            if (pKeyA && !keyA)
            {
                caller.setFlag(flag, option);
                p.popState();
            }
            if (pKeyB && !keyB)
            {
                caller.setFlag(flag, -1);
                p.popState();
            }
            if (pKeyUp && !keyUp)
            {
                option--;
                if (option == -1)
                    option = options.Length - 1;
            }
            if (pKeyDown && !keyDown)
            {
                option++;
                if (option == options.Length)
                    option = 0;
            }
            endMenu();
        }
    }

    class NumberPane : MenuState
    {
        int option = 0;
        int min, max;
        int yOffset;
        GameState caller;
        int flag;
        string message;
        TextBox textBox = null;
        int holdUp = 0, holdDown = 0;

        public NumberPane(GameState caller, int min, int max, int flag, string message, int yOffset, Player p) : base(p)
        {
            this.caller = caller;
            this.flag = flag;
            this.min = min;
            this.max = max;
            this.yOffset = yOffset;
            this.message = message;
            option = min;
            if (message != "")
            {
                textBox = new TextBox(caller, p, message);
                if (this.yOffset < 5)
                    this.yOffset += 5;
            }
        }

        public override void draw(Graphics g, Player p)
        {
            if (textBox != null) textBox.draw(g, p);
            int y = 15 - yOffset;
            Sprites.drawBox(g, 14, y, 6, 3);
            Sprites.drawString(g, "x" + option, 15, y + 1);
        }

        public override void run(Player p)
        {
            startMenu();
            if (pKeyA && !keyA)
            {
                caller.setFlag(flag, option);
                p.popState();
            }
            if (pKeyB && !keyB)
            {
                caller.setFlag(flag, -1);
                p.popState();
            }
            if (pKeyUp && (!keyUp || holdUp > 30))
            {
                option++;
                if (option > max)
                    option = min;
            }
            else if (pKeyUp) holdUp++; else holdUp = 0;
            if (pKeyDown && (!keyDown || holdDown > 30))
            {
                option--;
                if (option < min)
                    option = max;
            }
            else if (pKeyDown) holdDown++; else holdDown = 0;
            endMenu();
        }
    }

    class TextBox : MenuState
    {
        private int textLine = 0;
        private string[] lines = new string[0];
        GameState caller;

        public TextBox(GameState caller, Player player, string dialogue) : base(player)
        {
            this.caller = caller;
            addDialogue(splitDialogue(dialogue));
        }

        public override void draw(Graphics g, Player p)
        {
            caller.draw(g, p);
            Sprites.drawBox(g, 0, 13, 20, 5);
            if (lines.Length > textLine) Sprites.drawString(g, lines[textLine], 1, 14);
            if (lines.Length > textLine + 1)
                Sprites.drawString(g, lines[textLine + 1], 1, 16);
        }

        public override void run(Player p)
        {
            startMenu();
            if ((pKeyA && !keyA) || (pKeyB && !keyB))
            {
                if (lines.Length <= textLine + 2 || lines.Length <= 2)
                    p.popState();
                else
                {
                    textLine++;
                }
            }
            endMenu();
        }

        private void addDialogue(string[] s)
        {
            if (lines == null)
                lines = s;
            else
            {
                string[] newLines = new string[lines.Length + s.Length];
                for (int i = 0; i < lines.Length; i++)
                    newLines[i] = lines[i];
                for (int i = lines.Length; i < newLines.Length; i++)
                {
                    newLines[i] = s[i - lines.Length];
                }
                lines = newLines;
            }
        }

        private string[] splitDialogue(string s)
        {
            List<string> a = new List<string>();
            string[] ss = s.Split(' ');
            int i = 0;
            string b = "";
            while (i < ss.Length)
            {
                if (ss[i].Length + b.Length <= 18)
                {
                    b += ss[i];
                    if (b.Length != 18)
                        b += ' ';
                    i++;
                }
                else
                {
                    a.Add(b);
                    b = "";
                }
            }
            if (b != "")
                a.Add(b);
            return a.ToArray();
        }
    }

    class KeyboardDialogue : MenuState
    {
        GameState caller;
        int flag, length;
        char[] chars;
        int position;
        string message;
        const int CHARS_PER_LINE = 18, CHAR_LIMIT = 64;
        int option = 0;
        int frames = 0;

        public KeyboardDialogue(GameState caller, string message, int flag, int length, Player p) : base(p)
        {
            this.message = message;
            this.caller = caller;
            this.flag = flag;
            this.length = length;
            chars = new char[length];
        }

        static Pen pen = new Pen(Color.Gray);

        public override void draw(Graphics g, Player p)
        {
            g.Clear(Color.White);
            Sprites.drawString(g, message, 0, 0);
            for(int i = 0; i < position; i++)
            {
                Sprites.drawString(g, chars[i].ToString(), i, 1);
            }
            if((frames/30)%2==0)
            {
                Sprites.drawString(g, "_", position, 1);
            }
            for(int i = 0; i < CHAR_LIMIT; i++)
            {
                Sprites.drawString(g, getChar(i).ToString(), i % CHARS_PER_LINE + 1, i / CHARS_PER_LINE + 3);
            }
            g.DrawRectangle(pen, (option % CHARS_PER_LINE + 1) * 8, (option/CHARS_PER_LINE + 3) * 8, 8, 8);
            Sprites.drawString(g, "START to accept", 0, 17);
        }

        public override void run(Player p)
        {
            frames++;
            startMenu();
            if(pKeyB&&!keyB)
            {
                if (position == 0)
                {
                    caller.setFlag(flag, -1);
                    p.popState();
                } else
                {
                    position--;
                    chars[position] = ' ';
                }
            } else if(pKeyA&&!keyA)
            {
                if(position+1<length)
                {
                    chars[position] = getChar(option);
                    position++;
                }
            } else if(pKeyUp&&!keyUp)
            {
                if (option >= CHARS_PER_LINE)
                    option -= CHARS_PER_LINE;
            } else if(pKeyDown&&!keyDown)
            {
                if(option+CHARS_PER_LINE<CHAR_LIMIT)
                {
                    option += CHARS_PER_LINE;
                }
            } else if(pKeyLeft&&!keyLeft)
            {
                if(option%CHARS_PER_LINE!=0)
                {
                    option--;
                }
            } else if(pKeyRight&&!keyRight)
            {
                if(option%CHARS_PER_LINE!=CHARS_PER_LINE-1&&option+1!=CHAR_LIMIT)
                {
                    option++;
                }
            } else if(pKeyStart&&!keyStart)
            {
                for (int i = 0; i < position; i++)
                    caller.setFlag(flag + i + 1, chars[i]);
                caller.setFlag(flag, position);
                p.popState();
            }
            endMenu();
        }

        char getChar(int i)
        {
            if (i >= 0 && i < 26)
                return (char)('A' + i);
            if (i >= 26 && i < 52)
                return (char)('a' + i - 26);
            if (i >= 52 && i < 62)
                return (char)('0' + i - 52);
            return '?';
        }

        public static string getString(int [] flags, int start, int length)
        {
            StringBuilder s = new StringBuilder();
            for(int i = start; i < start+length; i++)
            {
                s.Append((char)flags[i]);
            }
            return s.ToString();
        }
    }

    class ColorPicker : MenuState
    {
        GameState caller;
        int flag;
        int option;
        int[] elements = new int[3];

        public ColorPicker(GameState caller, int flag, int r, int g, int b, Player p) : base(p)
        {
            this.caller = caller;
            this.flag = flag;
            elements[0] = r;
            elements[1] = g;
            elements[2] = b;
        }

        static Pen pen = new Pen(Color.Gold);

        public override void draw(Graphics g, Player p)
        {
            g.Clear(Color.White);
            for (int i = 0; i < 3; i++)
            {
                int temp = elements[i];
                for(int j = 0; j < 255; j += 2)
                {
                    elements[i] = j;
                    g.DrawLine(new Pen(Color.FromArgb(elements[0], elements[1], elements[2])), j / 2 + 6, 6 + 12 * i, j / 2 + 6, 12 + 12 * i);
                }
                elements[i] = temp;
            }
            g.DrawLine(pen, elements[option] / 2 + 6, 4 + option * 12, elements[option] / 2 + 6, 14 + option * 12);
        }

        public override void run(Player p)
        {
            startMenu();
            if(pKeyA&&!keyA)
            {
                for (int i = 0; i < 3; i++)
                    caller.setFlag(flag + i, elements[i]);
                p.popState();
            } else if(pKeyB&&!keyB)
            {
                caller.setFlag(flag, -1);
                p.popState();
            } else if(pKeyLeft)
            {
                if (elements[option] > 0) elements[option] -= 2;
            } else if(pKeyRight)
            {
                if (elements[option] < 254) elements[option] += 2;
            } else if(pKeyUp&&!keyUp)
            {
                if (option > 0) option--;
            } else if(pKeyDown&&!keyDown)
            {
                if (option < 2) option++;
            }
            endMenu();
        }
    }
}
