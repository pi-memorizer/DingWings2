using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using GameSystem;

namespace The_Revenge
{
    partial class Form1 : Form
    {
        public static int screenWidth = 160, screenHeight = 144; //resolution per player screen

        Bitmap offscreenImg = new Bitmap(screenWidth, screenHeight); //for double buffering
        Graphics offscreenG;

        public static IDictionary<string, int> config = new Dictionary<string, int>(); //so far just stores key mappings

        public Form1()
        {
            InitializeComponent();
            offscreenG = Graphics.FromImage(offscreenImg);

            //player 1 key mappings
            config.Add("p1Up", 'W'); //adds the option to set the player 1 up key
            config.Add("p1Down", 'S');
            config.Add("p1Left", 'A');
            config.Add("p1Right", 'D');
            config.Add("p1A", 'Q');
            config.Add("p1B", 'E');
            config.Add("p1Start", 'C');
            config.Add("p1Select", 'F');

            //player 2 key mappings
            config.Add("p2Up", 'I'); //these are technically here but are not used
            config.Add("p2Down", 'K');
            config.Add("p2Left", 'J');
            config.Add("p2Right", 'L');
            config.Add("p2A", 'U');
            config.Add("p2B", 'O');
            config.Add("p2Start", 'M');
            config.Add("p2Select", 'H');

            try
            {
                using (StreamReader sr = new StreamReader("config.txt"))
                {
                    string line = sr.ReadLine();
                    while(line!=null)
                    {
                        string[] s = line.Split('=');
                        if(s.Length==2)
                        {
                            if(config.ContainsKey(s[0]))
                            {
                                config[s[0]] = s[1][0];
                            } else
                            {
                                config.Add(s[0], s[1][0]);
                            }
                        }
                        line = sr.ReadLine();
                    }
                }
            } catch(IOException)
            {
                Console.WriteLine("There was an error reading the config file, so default settings will be used.");
            }
        }

        //where the scaling magic happens, and what is drawn to the form directly
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //to make it look pixelated
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            g.Clear(Color.Black);
            PictureBox pic = (PictureBox)sender;
            int width = pic.Width;
            int height = pic.Height;
            float wRat = width / (float)screenWidth;
            float hRat = height / (float)screenHeight;
            foreach (Player p in Game.players)
                p.draw();
            if (Game.players.Length == 2)
            {
                Player player = Game.players[0];
                Player player2 = Game.players[1];
                width /= 2;
                wRat /= 2;
                if (wRat < hRat)
                {
                    int newHeight = (int)(width / (float)screenWidth * (float)screenHeight);
                    g.DrawImage(player.offscreenImg, 0, (height - newHeight) / 2, width, newHeight);
                    g.DrawImage(player2.offscreenImg, width, (height - newHeight) / 2, width, newHeight);
                }
                else
                {
                    int newWidth = (int)(height / 144f * 160f);
                    g.DrawImage(player.offscreenImg, (width - newWidth) / 2, 0, newWidth, height);
                    g.DrawImage(player2.offscreenImg, (width - newWidth) / 2 + width, 0, newWidth, height);
                }
            }
            else if (Game.players.Length == 1)
            {
                Player player = Game.players[0];
                if (wRat < hRat)
                {
                    int newHeight = (int)(width / 160f * 144f);
                    g.DrawImage(player.offscreenImg, 0, (height - newHeight) / 2, width, newHeight);
                }
                else
                {
                    int newWidth = (int)(height / 144f * 160f);
                    g.DrawImage(player.offscreenImg, (width - newWidth) / 2, 0, newWidth, height);
                }
            }
            else if(Game.players.Length==0)
            {
                g.Clear(Color.White);
                Sprites.drawString(g, "Key bindings: ",1,1);
                int i = 3;
                foreach (string s in config.Keys)
                {
                    Sprites.drawString(g,s + ": ",1,i);
                    Sprites.drawString(g, ((char)config[s]).ToString(), 15, i);
                    i++;
                }
            }
            else
            {
                //fill out if we have more than 2 players
            }
        }

        //ticks at about 60 fps
        private void timer1_Tick(object sender, EventArgs e)
        {
            Game.run(); //game logic called
            Refresh(); //screen redrawn
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Program.setKey(e.KeyValue, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Program.setKey(e.KeyValue, false);
        }
    }
}
