using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameSystem
{
    class WorldEditor : WorldState
    {
        public World world;
        public bool tiles = false;
        public int block = (int)Block.Air;
        public WorldEditor(Player p, int worldID) : base(p)
        {
            world = WorldState.worlds[worldID];
            p.worldID = worldID;
        }

        public override void draw(Graphics g, Player p)
        {
            g.Clear(Color.Black);
            base.draw(g, p);
            Sprites.drawStringSmall(g, p.x + "," + p.y, 0, 0);
        }

        public override void run(Player p)
        {
            startMenu();
            if(pKeyLeft&&!keyLeft)
            {
                p.dir = 2;
                if (pKeyStart)
                {
                    p.x -= 16;
                }
                else
                {
                    p.x--;
                }
            }
            else
            if (pKeyRight&&!keyRight)
            {
                p.dir = 0;
                if (pKeyStart)
                {
                    p.x += 16;
                } else
                {
                    p.x++;
                }
            }
            else
            if (pKeyUp&&!keyUp)
            {
                p.dir = 1;
                if(pKeyStart)
                {
                    p.y -= 16;
                } else
                {
                    p.y--;
                }
            }
            else
            if (pKeyDown&&!keyDown)
            {
                p.dir = 3;
                if(pKeyStart)
                {
                    p.y += 16;
                } else
                {
                    p.y++;
                }
            } else if(pKeySelect&&!keySelect)
            {
                p.pushState(new EditorMenu(p,this));
            }
            else if(pKeyA&&!keyA)
            {
                p.forcePlace(0, 0, block, tiles);
            } else if(pKeyB&&!keyB)
            {
                p.forceMine(0, 0, tiles);
            }
            endMenu();
        }
    }

    class EditorMenu : MenuState
    {
        WorldEditor w;
        int option = 0;
        Pen pen = new Pen(Color.Black);
        int cpIndex = 0;
        static string[] cpMessages =
        {
            "Enter source x coord",
            "Enter source y coord",
            "Enter source width",
            "Enter source height",
            "Enter destination x coord",
            "Enter destination y coord"
        };

        public EditorMenu(Player p, WorldEditor w) : base(p)
        {
            this.w = w;
            flags = new int[6];
            for (int i = 0; i < flags.Length; i++)
                flags[i] = -2;
        }

        public override void draw(Graphics g, Player p)
        {
            g.Clear(Color.White);
            Sprites.drawString(g, "Tiles: " + w.tiles, 0, 0);
            Sprites.drawString(g, "Change tile/block", 0, 1);
            Sprites.drawString(g, "Copy block", 0, 2);
            Sprites.drawString(g, "Save world", 0, 3);
            Sprites.drawString(g, "Copy/Paste", 0, 4);
            Sprites.drawString(g, "Exit menu", 0, 5);
            g.DrawRectangle(pen, 0, option * 8 - 1, 159, 10);
        }

        public override void run(Player p)
        {
            startMenu();
            if(flags[0]==-2)
            {
                if(pKeyUp&&!keyUp)
                {
                    option--;
                    if (option == -1)
                        option = 5;
                } else if(pKeyDown&&!keyDown)
                {
                    option++;
                    if (option == 6)
                        option = 0;
                } else
                if(pKeyA&&!keyA)
                {
                    if (option == 0)
                    {
                        w.tiles = !w.tiles;
                    } else if(option==1)
                    {
                        p.pushState(new NumberPane(this, 0, WorldState.NUM_TILES, 0, "Set to ID", 0, p));
                    }
                    else if (option == 2)
                    {
                        if(w.tiles)
                        {
                            w.block = w.world.getTileAt(p.x, p.y);
                        } else
                        {
                            w.block = w.world.getBlockAt(p.x, p.y);
                        }
                    }
                    else if(option==3)
                    {
                        w.world.save(null);
                    } else if(option==4)
                    {
                        p.pushState(new NumberPane(this, 0, 999, 0, cpMessages[0], 0, p));
                    }
                    else if (option == 5)
                    {
                        p.popState();
                    }
                } else if(pKeyB&&!keyB)
                {
                    p.popState();
                }
            } else
            {
                if(option==1)
                {
                    w.block = flags[0];
                    flags[0] = -2;
                } else if(option==4)
                {
                    cpIndex++;
                    if(cpIndex==6)
                    {
                        cpIndex = 0;
                        int sx = flags[0];
                        int sy = flags[1];
                        int width = flags[2];
                        int height = flags[3];
                        int[,,] buffer = new int[width, height,2];
                        int dx = flags[4];
                        int dy = flags[5];
                        try
                        {
                            for (int x = sx; x < sx + width; x++)
                            {
                                for (int y = sy; y < sy + height; y++)
                                {
                                    buffer[x - sx, y - sy, 0] = w.world.getBlockAt(x, y);
                                    buffer[x - sx, y - sy, 1] = w.world.getTileAt(x, y);
                                }
                            }
                            for (int x = 0; x < width; x++)
                            {
                                for (int y = 0; y < height; y++)
                                {
                                    w.world.setBlockAt(x + dx, y + dy, buffer[x, y, 0]);
                                    w.world.setTileAt(x + dx, y + dy, buffer[x, y, 1]);
                                }
                            }
                            for (int i = 0; i < flags.Length; i++)
                                flags[i] = -2;
                        } catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        //copy and pasting
                    } else
                    {
                        if (flags[cpIndex - 1] == -1)
                        {
                            for (int i = 0; i < flags.Length; i++)
                                flags[i] = -2;
                            cpIndex = 0;
                        } else 
                        p.pushState(new NumberPane(this, 0, 999, cpIndex, cpMessages[cpIndex], 0, p));
                    }
                }
            }
            endMenu();
        }
    }
}
