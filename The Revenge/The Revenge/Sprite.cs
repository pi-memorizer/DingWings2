using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GameSystem
{
    //used for drawing text to the screen
    class Sprites
    {
        //private static Bitmap ascii = new Bitmap(new Bitmap("ascii.png"));
        private static Bitmap asciiSmall = new Bitmap(new Bitmap("ascii_small.png"));
        static Bitmap font = new Bitmap(new Bitmap("font.png"));

        public static void drawString(Graphics g, string s, int x, int y)
        {
            x *= 8;
            y *= 8;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                g.DrawImage(font, new RectangleF(x + i * 8, y,8,8), new RectangleF((c % 16) * 8, (c / 16) * 8, 8, 8), GraphicsUnit.Pixel);
            }
        }

        public static void drawStringSmall(Graphics g, string s, int x, int y)
        {
            x *= 4;
            y *= 6;
            s = s.ToUpper();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                g.DrawImage(asciiSmall, new RectangleF(x + i * 4, y+3,4,6), new RectangleF((c % 16) * 4, (c / 16) * 6, 4, 6), GraphicsUnit.Pixel);
            }
        }

        /*
         * top-left 0
         * top-middle 1
         * top-right 2
         * vertical-left 3
         * space 4
         * vertical-right 5
         * bottom-left 6
         * bottom-middle 7
         * bottom-right 8
         */
        private static char[] type0 = //standard box
        {
            'Ú',(char)('Ä'-1),'¿','³',' ',(char)('³'+1),'À','Ä','Ù'
        };

        private static char[] type1 = //box to link with other box on its right side
        {
            'Â','³','¿','Ä',' ','Á','Ù'
        };

        private static char[][] types =
        {
            type0
        };

        public static void drawBox(Graphics g, int left, int top, int width, int height, int type)
        {
            char[] t = types[type];

            for (int x = left; x < left + width; x++)
            {
                for (int y = top; y < top + height; y++)
                {
                    char c = t[4];
                    if (x == left)
                    {
                        if (y == top)
                            c = t[0];
                        else if (y == top + height - 1)
                            c = t[6];
                        else
                            c = t[3];
                    }
                    else if (x == left + width - 1)
                    {
                        if (y == top)
                            c = t[2];
                        else if (y == top + height - 1)
                            c = t[8];
                        else
                            c = t[5];
                    }
                    else if (y == top) {
                        c = t[1];
                    }
                    else if (y == top + height - 1)
                    {
                        c = t[7];
                    }
                    g.DrawImage(font, new RectangleF(x * 8, y * 8, 8, 8), new RectangleF((c % 16) * 8, (c / 16) * 8, 8, 8), GraphicsUnit.Pixel);
                }
            }
        }

        public static void drawBox(Graphics g, int left, int top, int width, int height)
        {
            drawBox(g, left, top, width, height, 0);
        }

    }

    //holds a flippable, sometimes palette switching sprite
    class Sprite
    {
        public Image sprite;
        Image flipped = null;
        public int xOffset = 0, yOffset = 0;
        public static ImageAttributes[] transparency;

        static Sprite()
        {
            transparency = new ImageAttributes[101];
            for (int i = 0; i < 101; i++)
            {
                transparency[i] = new ImageAttributes();
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = i / 100f;
                transparency[i].SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            }
        }

        private Sprite()
        {

        }

        public Sprite(string file) : this()
        {
            sprite = new Bitmap(new Bitmap(file)); //because for some reason it breaks otherwise
        }

        public Sprite(Bitmap spritesheet, int left, int top, int width, int height) : this()
        {
            Rectangle rect = new Rectangle(left, top, width, height);
            sprite = new Bitmap(spritesheet.Clone(rect, spritesheet.PixelFormat));
            //sprite = new Bitmap(sprite);
        }

        public Sprite(string file, int x, int y) : this()
        {
            sprite = new Bitmap(new Bitmap(file));
            xOffset = x;
            yOffset = y;
        }

        public Sprite(Bitmap spritesheet, int left, int top, int width, int height, int x, int y) : this()
        {
            Rectangle rect = new Rectangle(left, top, width, height);
            sprite = new Bitmap(spritesheet.Clone(rect, spritesheet.PixelFormat));
            xOffset = x;
            yOffset = y;
        }

        public int width
        {
            get
            {
                return sprite.Width;
            }
        }

        public int height
        {
            get
            {
                return sprite.Height;
            }
        }

        public void draw(Graphics g, int x, int y)
        {
            g.DrawImage(sprite, x + xOffset, y + yOffset);
        }

        public void drawFlipped(Graphics g, int x, int y)
        {
            if (flipped == null)
            {
                flipped = new Bitmap(sprite);
                flipped.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            g.DrawImage(flipped, x + xOffset, y + yOffset);
        }

        public void draw(Graphics g, int x, int y, int alpha)
        {
            Rectangle rect2 = new Rectangle(x + xOffset, y + yOffset, sprite.Width, sprite.Height);
            g.DrawImage(sprite, rect2, 0, 0, sprite.Width, sprite.Height, GraphicsUnit.Pixel, transparency[alpha]);
        }

        public void setPalette(ColorPalette c)
        {
            sprite.Palette = c;
            if (flipped != null)
            {
                sprite.Palette = c;
            }
        }

        public ColorPalette getPalette()
        {
            return sprite.Palette;
        }

        public static Sprite fromIndexed(string filename, bool isResource)
        {
            try
            {
                Sprite s = new Sprite();
                if (isResource)
                {
                    s.sprite = (Image)The_Revenge.Properties.Resources.ResourceManager.GetObject(filename);
                }
                else
                {
                    s.sprite = Image.FromFile(filename);
                }
                return s;
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return new Sprite("skeledog.bmp");
        }

        public static Sprite fromIndexed(string filename)
        {
            return fromIndexed(filename, false);
        }
    }
}
