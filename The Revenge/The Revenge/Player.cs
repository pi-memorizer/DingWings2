using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using The_Revenge;

namespace GameSystem
{
    class Player
    {
        //per player screen buffers
        public Bitmap offscreenImg = new Bitmap(Form1.screenWidth, Form1.screenHeight);
        Graphics g;

        //complicated, don't worry 'bout it
        public delegate bool GET_KEY(string key);
        public static GET_KEY getKeyCallback;

        public int id; //the player's id (1,2,etc)
        private GameState state; //the current state
        private Stack<GameState> stateStack = new Stack<GameState>(); //the stack of states, for state layering
        public int x = 16, y = 16, xOffset = 0, yOffset = 0; //position values
        public int worldID = 0; //the id of the world that the player is in
        public int wait = 0; //how many frames the player has waited since turning
        public int dir = 3; //the direction the player is facing, 0=right,1=up,2=left,3=down
        public bool isMale = false;
        static Sprite[] male, female; //sprites for multiple player types, will likely be changed
        static Bitmap maleSheet = new Bitmap("male.png"), femaleSheet = new Bitmap("female.png"); //the spritesheets for the player

        public World world
        {
            get
            {
                return WorldState.worlds[worldID];
            }
        }

        static Player()
        {
            //initializes the spritesheets
            male = new Sprite[12];
            for (int i = 0; i < 12; i++)
            {
                male[i] = new Sprite(maleSheet, (i % 4) * 16, (i / 4) * 24, 16, 24, 0, -8);
                if (i > 3)
                    male[i].yOffset = -9;
            }
            female = new Sprite[12];
            for (int i = 0; i < 12; i++)
            {
                female[i] = new Sprite(femaleSheet, (i % 4) * 16, (i / 4) * 24, 16, 24, 0, -8);
                if (i > 3)
                    female[i].yOffset = -9;
            }
        }

        public Player(int id)
        {
            g = Graphics.FromImage(offscreenImg);
            this.id = id;

            //code for world editing
            if(Game.args.Length==0)
            {
                state = new StartState(this);
            } else
            {
                int worldId = Convert.ToInt32(Game.args[0]);
                World s = WorldState.worlds[worldID];
                if(Game.args.Length==3)
                {
                    int x = Convert.ToInt32(Game.args[1]);
                    int y = Convert.ToInt32(Game.args[2]);
                } else
                {
                }
                s.load(null);
                s.editing = true;
                state = new WorldEditor(this, worldID);
            }
        }

        public void pushState(GameState s)
        {
            stateStack.Push(state);
            state = s;
        }

        public void popState()
        {
            state = stateStack.Pop();
            if (state is MenuState)
            {
                ((MenuState)state).startMenu();
                ((MenuState)state).endMenu();
            }
        }

        public void setState(GameState s)
        {
            stateStack.Clear();
            state = s;
        }
        
        //h3h3, probably won't use but it's here
        public bool canGoUnderwater()
        {
            return true;
        }

        public GameState getState()
        {
            return state;
        }

        public void draw()
        {
            state.draw(g, this);
        }

        public void run()
        {
            state.run(this);
        }

        //possible parameters are key names, i.e. "Up", "Left", "A", "Start", etc.
        public bool getKey(string key)
        {
            return getKeyCallback("p" + id + key);
        }

        //changes the block or tile at a position in the world
        public void forcePlace(int _x, int _y, int block, bool tile)
        {
            if (tile)
            {
                world.setTileAt(x + _x, y + _y, block);
            }
            else
            {
                world.setBlockAt(x + _x, y + _y, block);
            }
        }
        
        //removes the block or tile at a position in the world
        public void forceMine(int _x, int _y, bool tile)
        {
            if (tile)
            {
                world.setTileAt(x + _x, y + _y, (int)Block.Cloud);
            }
            else
            {
                world.setBlockAt(x + _x, y + _y, (int)Block.Air);
            }
        }

        //all the math is for animation of the sprite
        public Sprite getSprite()
        {
            int i = (xOffset != 0) ? xOffset : yOffset;
            if (i < 0) i *= -1;
            int j = x + y;
            if (j < 0) j *= -1;
            int s = dir;
            if (i >= 4 && i < 12)
            {
                s += 4;
                if (dir % 2 == 1)
                    s += (j % 2) * 4;
            }
            if (isMale)
                return male[s];
            else
                return female[s];
        }
    }
}
