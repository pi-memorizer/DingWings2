using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSystem
{
    class DeathState : MenuState
    {
        static Sprite deathScreen = new Sprite("gameover.png");
        string message;
        int fade = 1;
        
        public DeathState(Player p, string message) : base(p)
        {
            this.message = message;
            p.pushState(new TextBox(this, p, message));
            SoundSystem.setBackgroundMusic("emergency");
        }

        public override void draw(Graphics g, Player p)
        {
            deathScreen.draw(g, 0, 0);
            SolidBrush sb = new SolidBrush(Color.FromArgb(254-2*Math.Abs(fade), Color.DarkBlue));
            g.FillRectangle(sb, 0, 0, 160, 144);
        }

        public override void run(Player p)
        {
            startMenu();
            if(fade>0)
            {
                if(fade==126)
                {
                    p.pushState(new TextBox(this, p, message));
                    fade++;
                } else if(fade==127)
                {
                    if (pKeyA && !keyA)
                    {
                        fade = -127;
                    }
                } else
                {
                    fade++;
                }
            } else
            {
                if(fade==0)
                {
                    p.setState(new StartState(p));
                } else
                {
                    fade++;
                }
            }
            endMenu();
        }
    }
}
