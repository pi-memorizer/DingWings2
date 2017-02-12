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
        public static int WATER_SPEED = 60*150;
        public static int waterLevel = WATER_SPEED;
        const int WAIT = 3; //amount of frames to wait before moving after changing direction
        public int WALK_SPEED = 2; //how many pixels the player moves per frame
        static Sprite[] waterSprites, waterLevelSprites;
        public static Sprite bubble = new Sprite("bubble.png");

        public static int drainFrames = 0;

        public static int[] pipeUsesLeft;

        public WorldState(Player player) : base(player)
        {
            if (player.id == 2) player.isMale = false;
        }

        static WorldState()
        {
            tileSprites = new Sprite[NUM_TILES];
            //loads the tiles from images into their rightful places
            loadSegment("tileset1.png", 0, 256, 16, 16, 0, 0);
            loadSegment("tileset2.png", 256, 256, 16, 24, 0, -8);
            loadSegment("tileset3.png", 512, 65, 16, 24, 0, -8);

            //if we add more worlds make sure to add them here
            worldinit();

            Player.maleSheet = new Bitmap("guy.png");
            Player.femaleSheet = new Bitmap("guy2.png");
            Player.male = new Sprite[12];
            for (int i = 0; i < 12; i++)
            {
                Player.male[i] = new Sprite(Player.maleSheet, (i % 4) * 16, (i / 4) * 24, 16, 24, 0, -8);
                if (i > 3)
                    Player.male[i].yOffset = -9;
            }
            Player.female = new Sprite[12];
            for (int i = 0; i < 12; i++)
            {
                Player.female[i] = new Sprite(Player.femaleSheet, (i % 4) * 16, (i / 4) * 24, 16, 24, 0, -8);
                if (i > 3)
                    Player.female[i].yOffset = -9;
            }

            Bitmap maleScubaSheet = new Bitmap("guysnorkel.png");
            Player.maleScuba = new Sprite[12];
            for (int i = 0; i < 12; i++)
            {
                Player.maleScuba[i] = new Sprite(maleScubaSheet, (i % 4) * 16, (i / 4) * 24, 16, 24, 0, -8);
                if (i > 3)
                    Player.maleScuba[i].yOffset = -9;
            }
            Bitmap femaleScubaSheet = new Bitmap("guy2snorkel.png");
            Player.femaleScuba = new Sprite[12];
            for (int i = 0; i < 12; i++)
            {
                Player.femaleScuba[i] = new Sprite(femaleScubaSheet, (i % 4) * 16, (i / 4) * 24, 16, 24, 0, -8);
                if (i > 3)
                    Player.femaleScuba[i].yOffset = -9;
            }

            Bitmap water = new Bitmap("water.png");
            waterSprites = new Sprite[16];
            for(int i = 0; i < 16; i++)
            {
                waterSprites[i] = new Sprite(water, 0, 16 - i, 16, 16 + i);
                waterSprites[i].yOffset = -i;
            }
            waterLevelSprites = new Sprite[16];
            for(int i = 0; i < 16; i++)
            {
                waterLevelSprites[i] = new Sprite(water, 0, 16-i, 16, i+1, 0, 15 - i);
            }
        }

        public static void worldinit()
        {
            worlds = new Dictionary<int, World>();
            worlds.Add(0, new Map0(0, 32, 32));
            worlds.Add(1, new Map1(1, 32, 32));
            worlds.Add(2, new Map2(2, 32, 32));
            worlds.Add(3, new Map3(3, 32, 32));
            foreach (World w in worlds.Values)
            {
                w.load(null);
            }
            pipeUsesLeft = new int[] {
                3,
                3,
                3,
                3
            };
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

        public int getWaterPixel()
        {
            int waterPixel = 0;
            if (waterLevel / WATER_SPEED < p.level)
            {
                waterPixel = 16;
            }
            else if (waterLevel / WATER_SPEED == p.level)
            {
                waterPixel = 16-(16 * (waterLevel % WATER_SPEED) / WATER_SPEED);
            }
            return waterPixel;
        }

        //most if this is kind of complicated, and shouldn't need to be modified.
        //code exists already for if we want underwater features for some reason.
        public override void draw(Graphics g, Player p)
        {
            List<DrawUnit>[] layers = new List<DrawUnit>[19];
            int waterPixel = getWaterPixel();
            if (p.id == 1)
            {
                if(p.breath<Player.MAX_BREATH/2)
                {
                    SoundSystem.setBackgroundMusic("ohShiitakeMushrooms");
                } else if (waterPixel > 12)
                {
                    SoundSystem.setBackgroundMusic("emergency2");
                } else
                {
                    if (p.worldID == 0)
                        SoundSystem.setBackgroundMusic("beige");
                    else if (p.worldID == 1)
                        SoundSystem.setBackgroundMusic("dracula");
                    else if (p.worldID == 2)
                        SoundSystem.setBackgroundMusic("creepyMusic");
                    else if (p.worldID == 3)
                        SoundSystem.setBackgroundMusic("eeyore");
                }
            }

            for(int i = 0; i < 19; i++)
            {
                layers[i] = new List<DrawUnit>();
            }

            /*if (waterPixel==16)
            {
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
            else*/
            { 
                g.Clear(Color.Black);
                for (int x = p.x - 7; x <= p.x + 7; x++) //important! tiles get drawn on before blocks
                {
                    for (int y = p.y - 5; y <= p.y + 5; y++)
                    {
                        Sprite s;
                        int layer = y - p.y + 7;
                        int tile = p.world.getTileAt(x, y);
                        if (waterPixel > 0 && waterPixel < 16&&p.world.isInside(x,y)&&(tile!=(int)Block.UpStairs&&tile!=(int)Block.DownStairs))
                        {
                            s = waterSprites[0];
                            layer--;
                        }
                        else
                        {
                            s = tileSprites[tile];
                        }
                        layers[layer].Add(new DrawUnit(s, 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                    }
                }
                for (int i = 0; i < Game.players.Length; i++)
                {
                    Player p2 = Game.players[i];
                    if (p2.world == p.world)
                    {
                        p2.isScuba = waterPixel > 13;
                        int layer = p2.y - p.y + 9;
                        if (layer >= 0 && layer < layers.Length)
                            layers[layer].Add(new DrawUnit(p2.getSprite(), 72 + p2.x * 16 + p2.xOffset - p.x * 16 - p.xOffset, 64 + p2.y * 16 + p2.yOffset - p.y * 16 - p.yOffset));
                        if(waterPixel>0&&waterPixel<16)
                        {
                            layers[layer].Add(new DrawUnit(waterLevelSprites[waterPixel], 72 + p2.x * 16 + p2.xOffset - p.x * 16 - p.xOffset, 64 + p2.y * 16 + p2.yOffset - p.y * 16 - p.yOffset));
                        }
                    }
                }
                List<Entity> entities = p.world.entities;
                for (int i = 0; i < entities.Count; i++)
                {
                    Entity e = entities[i];
                    int layer = e.y - p.y + 9;
                    if (layer >= 0 && layer < layers.Length)
                        layers[layer].Add(new DrawUnit(e.getSprite(), 72 + 16 * e.x - p.x * 16 - p.xOffset + e.xOffset, 64 + 16 * e.y - p.y * 16 - p.yOffset + e.yOffset));
                }
                for (int x = p.x - 7; x <= p.x + 7; x++)
                {
                    for (int y = p.y - 5; y <= p.y + 6; y++)
                    {
                        int block = p.world.getBlockAt(x, y);
                        if (block == 0) {
                            if (!p.world.isInside(x, y)||y==0) continue;
                            int t = p.world.getTileAt(x, y);
                            if(p.world.getTileAt(x,y)!=0&&waterPixel<16&&waterPixel!=0&&t!=(int)Block.UpStairs&&t!=(int)Block.DownStairs)
                            {
                                //TODO draw various water levels
                                Sprite s = waterSprites[(waterPixel)];
                                if (s != null) layers[y - p.y + 8].Add(new DrawUnit(s, 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                            }
                        }
                        else
                        {
                            Sprite s = tileSprites[block];
                            if (s != null) layers[y - p.y + 8].Add(new DrawUnit(s, 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                            if (waterPixel > 0 && waterPixel < 16)
                            {
                                if(BlockData.canHaveWater(block))
                                {
                                    layers[y - p.y + 8].Add(new DrawUnit(waterLevelSprites[waterPixel], 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                                }
                                /*
                                int block2 = p.world.getBlockAt(x, y + 1);
                                if (block < 512)
                                {
                                    int block3 = p.world.getBlockAt(x, y - 1);
                                    if (block2 == 0&&(block3==0||block3>=512))
                                        layers[y - p.y + 9].Add(new DrawUnit(waterLevelSprites[waterPixel], 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                                } else if(block2<512)
                                {
                                    layers[y - p.y + 9].Add(new DrawUnit(waterLevelSprites[waterPixel], 72 - p.xOffset + 16 * (x - p.x), 64 - p.yOffset + 16 * (y - p.y)));
                                }*/
                            }
                        }
                    }
                }
            }
            for(int i = 0; i < layers.Length; i++)
            {
                foreach(DrawUnit d in layers[i])
                {
                    Sprite s = d.sprite;
                    s.draw(g, d.x, d.y);
                }
            }
            if(waterPixel==16)
            {
                SolidBrush sb = new SolidBrush(Color.FromArgb(128, Color.DarkBlue));
                g.FillRectangle(sb, 0, 0, 200, 144);
            }
            if(p.isScuba)
            {
                for(int i = 0; i < 8*p.breath/Player.MAX_BREATH; i++)
                {
                    bubble.draw(g, 2 + i * 10, 2);
                }
            }
        }

        //whether or not the player is permitted to move on a space
        public bool canMove(Player p, int x, int y)
        {
            Block t = (Block)p.world.getTileAt(x, y);
            Block b = (Block)p.world.getBlockAt(x, y);
            if (!p.world.isInside(x, y)) return false;
            bool flag = false;
            foreach(Entity e in p.world.entities)
            {
                if (e.collides(x, y, p.xOffset, p.yOffset, 16, 16)) return false;
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

        static int stairCount = 0;

        public override void run(Player p)
        {
            startMenu();
            int waterPixel = getWaterPixel();
            if (p.xOffset == 0 && p.yOffset == 0 && p.wait == 0) //if the player is ready to move again
            {
                {
                    Block b = (Block)p.world.getTileAt(p.x, p.y);
                    if(b==Block.UpStairs)
                    {
                        if (stairCount < 3)
                        {
                            SoundSystem.play("stairs");
                            stairCount++;
                        }
                        if (p.worldID==3)
                        {
                            p.dir = 3;
                            p.level--;
                            p.worldID--;
                            p.x = 6;
                            p.y = 1;
                            p.yOffset = WALK_SPEED-16;
                        } else
                        {
                            if (p.worldID == 1) p.y--;
                            p.dir = 2;
                            p.level--;
                            p.x--; p.xOffset = 16 - WALK_SPEED;
                            p.worldID--;
                        }
                        return;
                    }
                    if (b == Block.DownStairs)
                    {
                        if (stairCount < 3)
                        {
                            SoundSystem.play("stairs");
                            stairCount++;
                        }
                        if (p.worldID == 2)
                        {
                            p.level++;
                            p.x = 6;
                            p.y = 1;
                            p.worldID++;
                            p.yOffset = WALK_SPEED-16;
                            p.dir = 3;
                        }
                        else
                        {
                            p.level++;
                            p.x--;
                            if (p.worldID == 0) p.y++;
                            p.xOffset = 16 - WALK_SPEED;
                            p.dir = 2;
                            p.worldID++;
                        }
                        return;
                    }
                }
                if (pKeyLeft)
                {
                    if (p.dir == 2)
                    {
                        if (canMove(p, p.x - 1, p.y))
                        {
                            p.x--;
                            p.xOffset = 16-WALK_SPEED;
                            if (waterPixel == 0) SoundSystem.play("footsteps");
                            else if (waterPixel < 16)
                            {
                                SoundSystem.play("water");
                            }
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
                            if (waterPixel == 0) SoundSystem.play("footsteps");
                            else if (waterPixel < 16)
                            {
                                SoundSystem.play("water");
                            }
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
                            if (waterPixel == 0) SoundSystem.play("footsteps");
                            else if (waterPixel < 16)
                            {
                                SoundSystem.play("water");
                            }
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
                            if (waterPixel == 0) SoundSystem.play("footsteps"); else if(waterPixel<16)
                            {
                                SoundSystem.play("water");
                            }
                        }
                    }
                    else
                    {
                        p.dir = 3;
                        p.wait = WAIT;
                    }
                }
                if(pKeyA&&!keyA)
                {
                    int x = p.x;
                    int y = p.y;
                    switch (p.dir)
                    {
                        case 0: x++; break;
                        case 1: y--; break;
                        case 2: x--; break;
                        case 3: y++; break;
                    }
                    Entity e = p.world.entityAt(x, y);
                    if(e!=null)
                    {
                        e.interact(p);
                    }
                }
            } else
            {
                //slides the player into the grid, will need to be modified if we want to stray from a grid based system
                if (p.xOffset > 0) p.xOffset-=WALK_SPEED;
                if (p.xOffset < 0) p.xOffset+=WALK_SPEED;
                if (Math.Abs(p.xOffset) < WALK_SPEED) p.xOffset = 0;
                if (p.yOffset > 0) p.yOffset-=WALK_SPEED;
                if (p.yOffset < 0) p.yOffset+=WALK_SPEED;
                if (Math.Abs(p.yOffset) < WALK_SPEED) p.yOffset = 0;
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
