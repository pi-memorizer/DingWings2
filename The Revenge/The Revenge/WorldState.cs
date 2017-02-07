using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameSystem
{
    class WorldState : MenuState
    {
        public static int NUM_TILES = 1024;
        public static Sprite[] tileSprites = null;
        public static Dictionary<int, World> worlds = null;
        public static int numWorlds = 1;
        public static string worldName = "";
        const int WAIT = 3; //amount of frames to wait before moving after changing direction
        const int WALK_SPEED = 2; //how many pixels the player moves per frame

        public WorldState(Player player) : base(player)
        {
        }

        static WorldState()
        {
            tileSprites = new Sprite[NUM_TILES];
            //loads the tiles from images into their rightful places
            loadSegment("tileset1.png", 0, (int)Block.Coral+1, 16, 16, 0, 0);
            loadSegment("tileset2.png", 256, 9, 16, 24, 0, -8);
            loadSegment("tileset3.png", 512, 6, 16, 16, 0, 0);

            //if we add more worlds make sure to add them here
            worlds = new Dictionary<int, World>();
            worlds.Add(0,new StaticWorld(0,16,16));
            foreach(World w in worlds.Values)
            {
                w.load(null);
            }
        }

        //loads blocks from a sprite sheet
        private static void loadSegment(string name, int start, int length, int tileWidth, int tileHeight, int xOffset, int yOffset)
        {
            Bitmap sheet = new Bitmap(name);
            int per = sheet.Width / tileWidth;
            for(int i = start; i < start+length; i++)
            {
                tileSprites[i] = new Sprite(sheet, ((i-start) % per) * tileWidth, ((i-start) / per) * tileHeight, tileWidth, tileHeight, xOffset, yOffset);
            }
        }

        //for layering, don't touch
        protected class DrawUnit
        {
            public Sprite sprite;
            public int x, y;
            public DrawUnit(Sprite s, int x, int y)
            {
                sprite = s;
                this.x = x;
                this.y = y;
            }
        }

        //most if this is kind of complicated, and shouldn't need to be modified.
        //code exists already for if we want underwater features for some reason.
        public override void draw(Graphics g, Player p)
        {
            List<DrawUnit>[] layers = new List<DrawUnit>[19];
            for(int i = 0; i < 19; i++)
            {
                layers[i] = new List<DrawUnit>();
            }

            if (p.world.getTileAt(p.x, p.y) == (int)Block.DeepWater)
            {
                Sprite st = tileSprites[(int)Block.OceanFloor];
                g.Clear(Color.Black);
                for (int x = p.x - 7; x <= p.x + 7; x++) //important! tiles get drawn on before blocks
                {
                    for (int y = p.y - 5; y <= p.y + 5; y++)
                    {
                        if (p.world.getTileAt(x, y) == (int)Block.DeepWater)
                            if (st != null)
                            {
                                layers[y + 7 - p.y].Add(new DrawUnit(st, 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                            }
                    }
                }
                for(int i = 0; i < Game.players.Length; i++)
                {
                    Player p2 = Game.players[i];
                    if (p2.world == p.world)
                    {
                        if (p2.world.getTileAt(p2.x, p2.y) == (int)Block.DeepWater)
                        {
                            int layer = p2.y - p.y + 9;
                            if (layer >= 0 && layer < layers.Length)
                                layers[layer].Add(new DrawUnit(p2.getSprite(), 72 + p2.x * 16 + p2.xOffset - p.x * 16 - p.xOffset, 64 + p2.y * 16 + p2.yOffset - p.y * 16 - p.yOffset));
                        }
                    }
                }
                List<Entity> entities = p.world.entities;
                for (int i = 0; i < entities.Count; i++)
                {
                    Entity e = entities[i];
                    if (e.world.getTileAt(e.x, e.y) == (int)Block.DeepWater)
                    {
                        int layer = e.y - p.y + 9;
                        if (layer >= 0 && layer < layers.Length)
                            layers[layer].Add(new DrawUnit(e.getSprite(), 72 + 16 * e.x - p.x * 16 - p.xOffset, 64 + 16 * e.y - p.y * 16 - p.yOffset));
                    }
                }
                for (int x = p.x - 7; x <= p.x + 7; x++)
                {
                    for (int y = p.y - 5; y <= p.y + 6; y++)
                    {
                        if (p.world.getTileAt(x, y) != (int)Block.DeepWater) continue;
                        int block = p.world.getBlockAt(x, y);
                        if (block == 0) continue;
                        int layer = y - p.y + 8;
                        if (block == (int)Block.Kelp) layer++;
                        Sprite s = tileSprites[block];
                        if (s != null) layers[layer].Add(new DrawUnit(s, 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                    }
                }
            }
            else
            { 
                g.Clear(Color.Black);
                for (int x = p.x - 7; x <= p.x + 7; x++) //important! tiles get drawn on before blocks
                {
                    for (int y = p.y - 5; y <= p.y + 5; y++)
                    {
                        layers[y + 7 - p.y].Add(new DrawUnit(tileSprites[p.world.getTileAt(x, y)], 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                    }
                }
                for (int i = 0; i < Game.players.Length; i++)
                {
                    Player p2 = Game.players[i];
                    if (p2.world == p.world)
                    {
                        if (p2.world.getTileAt(p2.x, p2.y) != (int)Block.DeepWater)
                        {
                            int layer = p2.y - p.y + 9;
                            if (layer >= 0 && layer < layers.Length)
                                layers[layer].Add(new DrawUnit(p2.getSprite(), 72 + p2.x * 16 + p2.xOffset - p.x * 16 - p.xOffset, 64 + p2.y * 16 + p2.yOffset - p.y * 16 - p.yOffset));
                        }
                    }
                }
                List<Entity> entities = p.world.entities;
                for (int i = 0; i < entities.Count; i++)
                {
                    Entity e = entities[i];
                    if (e.world.getTileAt(e.x, e.y) != (int)Block.DeepWater)
                    {
                        int layer = e.y - p.y + 9;
                        if (layer >= 0 && layer < layers.Length)
                            layers[layer].Add(new DrawUnit(e.getSprite(), 72 + 16 * e.x - p.x * 16 - p.xOffset, 64 + 16 * e.y - p.y * 16 - p.yOffset));
                    }
                }
                for (int x = p.x - 7; x <= p.x + 7; x++)
                {
                    for (int y = p.y - 5; y <= p.y + 6; y++)
                    {
                        if (p.world.getTileAt(x, y) == (int)Block.DeepWater) continue;
                        int block = p.world.getBlockAt(x, y);
                        if (block == 0) continue;
                        Sprite s = tileSprites[block];
                        if (s != null) layers[y - p.y + 8].Add(new DrawUnit(s, 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                    }
                }
            }
            for(int i = 0; i < layers.Length; i++)
            {
                foreach(DrawUnit d in layers[i])
                {
                    d.sprite.draw(g, d.x, d.y);
                }
            }
            if(p.world.getTileAt(p.x,p.y)==(int)Block.DeepWater)
            {
                SolidBrush sb = new SolidBrush(Color.FromArgb(128, Color.DarkBlue));
                g.FillRectangle(sb, 0, 0, 160, 144);
            }
        }

        //whether or not the player is permitted to move on a space
        public bool canMove(Player p, int x, int y)
        {
            Block t = (Block)p.world.getTileAt(x, y);
            Block b = (Block)p.world.getBlockAt(x, y);
            bool flag = false;
            if (t == Block.Water || t == Block.DeepWater)
            {
                if (!p.canGoUnderwater())
                    return false;
                else flag = true;
                if (b == Block.Coral)
                    flag = false;
            }
            Entity e = p.world.entityAt(x, y);
            if(e!=null)
            {
                if (!e.getCanStepOn(p))
                    return false;
            }
            if (b == 0)
                return true;
            else
            {
                if (flag)
                    return true;
                return false;
            }
        }


        public override void run(Player p)
        {
            startMenu();
            if(p.xOffset==0&&p.yOffset==0&&p.wait==0) //if the player is ready to move again
            {
                if (pKeyLeft)
                {
                    if (p.dir == 2)
                    {
                        if (canMove(p, p.x - 1, p.y))
                        {
                            p.x--;
                            p.xOffset = 16-WALK_SPEED;
                        }
                    }
                    else
                    {
                        p.dir = 2;
                        p.wait = WAIT;
                    }
                }
                else
                if (pKeyRight)
                {
                    if (p.dir == 0)
                    {
                        if (canMove(p, p.x + 1, p.y))
                        {
                            p.x++;
                            p.xOffset = WALK_SPEED-16;
                        }
                    }
                    else
                    {
                        p.dir = 0;
                        p.wait = WAIT;
                    }
                }
                else
                if (pKeyUp)
                {
                    if (p.dir == 1)
                    {
                        if (canMove(p, p.x, p.y - 1))
                        {
                            p.y--;
                            p.yOffset = 16-WALK_SPEED;
                        }
                    }
                    else
                    {
                        p.dir = 1;
                        p.wait = WAIT;
                    }
                }
                else
                if (pKeyDown)
                {
                    if (p.dir == 3)
                    {
                        if (canMove(p, p.x, p.y + 1))
                        {
                            p.y++;
                            p.yOffset = WALK_SPEED-16;
                        }
                    }
                    else
                    {
                        p.dir = 3;
                        p.wait = WAIT;
                    }
                }
            } else
            {
                //slides the player into the grid, will need to be modified if we want to stray from a grid based system
                if (p.xOffset > 0) p.xOffset-=WALK_SPEED;
                if (p.xOffset < 0) p.xOffset+=WALK_SPEED;
                if (p.yOffset > 0) p.yOffset-=WALK_SPEED;
                if (p.yOffset < 0) p.yOffset+=WALK_SPEED;
                if (p.wait > 0) p.wait--;
            }
            if (p.xOffset == 0 && p.yOffset == 0 && p.getState() == this)
            {
                Entity e = p.world.entityAt(p.x, p.y);
                if(e!=null)
                {
                    e.onStepOn(p);
                }
            }
            endMenu();
        }
    }
}
