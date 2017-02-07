using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace GameSystem
{
    //"switching states, but like better" - Lily, 2/3/2017
    abstract class GameState
    {
        protected Player p; //the player that the state references
        protected int[] flags = new int[1]; //complicated, used for stacked menus, ask Kyle if you need it :P

        public GameState(Player player)
        {
            p = player;
        }

        public void setFlag(int index, int value)
        {
            if (index >= 0 && index < flags.Length)
                flags[index] = value;
        }
        
        //if this is a current state, you'll need a draw it
        public abstract void draw(Graphics g, Player p);

        //runs every frame if this state is active
        public abstract void run(Player p);
    }

    class StartState : MenuState
    {
        static Sprite splashScreen = new Sprite("splashscreen.png");

        public StartState(Player player) : base(player)
        {
        }

        public override void draw(Graphics g, Player p)
        {
            splashScreen.draw(g, 0, 0);
        }

        public override void run(Player p)
        {
            startMenu();
            if(pKeyStart&&!keyStart)
            {
                p.setState(new WorldState(p));
            }
            endMenu();
        }
    }
}
