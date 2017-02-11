using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GameSystem
{
    abstract class World
    {
        public List<Entity> entities = new List<Entity>();
        public World() { }
        public abstract int getBlockAt(int x, int y);
        public abstract bool setBlockAt(int x, int y, int block);
        public abstract int getTileAt(int x, int y);
        public abstract bool setTileAt(int x, int y, int block);
        public bool editing = false; //true only when using the world editor
        public abstract void load(BinaryReader br);
        public abstract void save(BinaryWriter bw);
        public Entity entityAt(int x, int y)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity e = entities[i];
                if (e.x == x && e.y == y)
                    return e;
            }
            return null;
        }
        public virtual void tick(Player p)
        {

        }

        public abstract bool isInside(int x, int y);
    }

    public struct Coord
    {
        public int x, y;
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Coord)
            {
                Coord c = (Coord)obj;
                return c.x == x && c.y == y;
            }
            else return false;
        }
        public override int GetHashCode()
        {
            return x ^ y;
        }
    }

    class StaticWorld : World
    {
        public int id = 0;
        public int width = -1, height = -1;

        ushort[,] blocks, tiles;

        public StaticWorld()
        {
            if (blocks == null)
            {
                if (width == -1)
                {
                    width = 16;
                    height = 16;
                }
                blocks = new ushort[width, height];
                tiles = new ushort[width, height];
            }
        }

        public StaticWorld(int id) : this()
        {
            this.id = id;
        }

        public StaticWorld(int id, int width, int height) : this(id)
        {
            this.width = width;
            this.height = height;
        }

        public override int getBlockAt(int x, int y)
        {
            if (blocks == null) return 0;
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return blocks[x, y];
            }
            else return 0;
        }

        public override int getTileAt(int x, int y)
        {
            if (tiles == null) return 0;
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return tiles[x, y];
            }
            else return 0;
        }

        public override bool setBlockAt(int x, int y, int block)
        {

            if (editing)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    blocks[x, y] = (ushort)block;
                }
            }
            return editing;
        }

        public override bool setTileAt(int x, int y, int block)
        {
            if (editing)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    tiles[x, y] = (ushort)block;
                }
            }
            return editing;
        }

        public override void load(BinaryReader br)
        {
            try
            {
                BinaryReader r = new BinaryReader(new FileStream("data/" + id + ".smap", FileMode.Open));
                int _width = width;
                int _height = height;
                width = r.ReadInt32();
                height = r.ReadInt32();
                if (_width != -1)
                {
                    blocks = new ushort[_width, _height];
                    tiles = new ushort[_width, _height];
                }
                else
                {
                    blocks = new ushort[width, height];
                    tiles = new ushort[width, height];
                }
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        blocks[x, y] = r.ReadUInt16();
                        tiles[x, y] = r.ReadUInt16();
                    }
                }
                if (_width != -1)
                {
                    width = _width;
                    height = _height;
                }
                r.Close();
            }
            catch (IOException)
            {
                if (width == -1)
                {
                    width = 32;
                    height = 32;
                }
                blocks = new ushort[width, height];
                tiles = new ushort[width, height];

            }
        }

        public override void save(BinaryWriter bw)
        {
            if (bw != null)
            {
                //for the actual saving
            }
            else
            {
                //for the special haxoring saving
                try
                {
                    BinaryWriter w = null;
                    try
                    {
                        w = new BinaryWriter(new FileStream(WorldState.worldName + "data/" + id + ".smap", FileMode.CreateNew));
                    }
                    catch (IOException)
                    {
                        w = new BinaryWriter(new FileStream(WorldState.worldName + "data/" + id + ".smap", FileMode.Create));
                    }
                    w.Write(width);
                    w.Write(height);
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            w.Write(blocks[x, y]);
                            w.Write(tiles[x, y]);
                        }
                    }
                    w.Close();
                }
                catch (IOException ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public override bool isInside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }
    }
}
