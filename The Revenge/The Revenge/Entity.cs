using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSystem
{
    //anything that moves or changes
    abstract class Entity
    {
        public int x, y, xOffset, yOffset; //coordinates
        public World world; //the world it is in
        protected bool remove = false; //set true to take it out of the system

        public Entity(World world, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.world = world;
        }

        public Entity(World world)
        {
            this.world = world;
        }

        public abstract Sprite getSprite();
        public abstract void tick(); //called every frame (60 fps)
        public virtual bool collides(int x, int y, int xOffset, int yOffset, int width, int height)
        {
            return x == this.x && y == this.y;
        }
        public abstract void onStepOn(Player p); //called when the player steps on it (i.e. land mines)
        public virtual bool needsRemoval() { return remove; } //called to check if the entity needs removal
        public abstract bool interact(Player p); //called if the player selects the entity, true if interaction was made
        public void forceRemove()
        {
            remove = true;
        }
    }

    class Particle : Entity
    {
        Sprite[] sprites;
        int lifetime;
        int frames = 0;

        public Particle(World world, int x, int y, Sprite[] sprites, int lifetime) : base(world,x,y)
        {
            this.sprites = sprites;
            this.lifetime = lifetime;
        }

        public override bool collides(int x, int y, int xOffset, int yOffset, int width, int height)
        {
            return false;
        }

        public override Sprite getSprite()
        {
            return sprites[frames * sprites.Length / lifetime];
        }

        public override bool interact(Player p)
        {
            return false;
        }

        public override bool needsRemoval()
        {
            return frames >= lifetime;
        }

        public override void onStepOn(Player p)
        {
        }

        public override void tick()
        {
            frames++;
        }
    }

    class Pushable : Entity
    {
        Sprite sprite;
        public Pushable(Sprite s, World w, int x, int y) : base(w,x,y)
        {
            sprite = s;
        }

        public override Sprite getSprite()
        {
            return sprite;
        }

        public override bool interact(Player p)
        {
            if (xOffset != 0 || yOffset != 0) return false;
            int x = this.x;
            int y = this.y;
            switch(p.dir)
            {
                case 0: x++; break;
                case 1: y--; break;
                case 2: x--; break;
                case 3: y++; break;
            }
            if(world.entityAt(x,y)==null)
            {
                if(world.getBlockAt(x,y)==0&&world.getTileAt(x,y)!=0)
                {
                    switch (p.dir)
                    {
                        case 0:
                            this.x++;
                            xOffset = -15;
                            break;
                        case 1:
                            this.y--;
                            yOffset = 15;
                            break;
                        case 2:
                            this.x--;
                            xOffset = 15;
                            break;
                        case 3:
                            this.y++;
                            yOffset = -15;
                            break;
                    }
                }
            }
            return true;
        }

        public override void onStepOn(Player p)
        {}

        public override void tick()
        {
            if (xOffset < 0) xOffset++;
            if (xOffset > 0) xOffset--;
            if (yOffset < 0) yOffset++;
            if (yOffset > 0) yOffset--;
        }
    }

    delegate void GameEvent(Player p, Entity e);

    class WorldItem : Entity
    {
        Sprite sprite;
        Player.Item item;
        string name;
        public WorldItem(Sprite s, Player.Item i, string name, World w, int x, int y) : base(w,x,y)
        {
            item = i;
            sprite = s;
            this.name = name;
        }

        public override Sprite getSprite()
        {
            return sprite;
        }

        public override bool interact(Player p)
        {
            p.items[(int)item] = true;
            remove = true;
            p.pushState(new TextBox(p.getState(), p, "You got the " + name + "!"));
            return true;
        }

        public override void onStepOn(Player p)
        {}

        public override void tick()
        {}
    }

    class MessageEntity : Entity
    {
        Sprite sprite;
        string message;

        public MessageEntity(Sprite s, string message, World w, int x, int y) : base(w,x,y)
        {
            sprite = s;
            this.message = message;
        }

        public override Sprite getSprite()
        {
            return sprite;
        }

        public override bool interact(Player p)
        {
            p.pushState(new TextBox(p.getState(), p, message));
            return true;
        }

        public override void onStepOn(Player p)
        {}

        public override void tick()
        {}
    }

    class EventEntity : Entity
    {
        Sprite sprite;
        string message;
        GameEvent e;

        public EventEntity(Sprite s, string message, GameEvent e, World w, int x, int y) : base(w,x,y)
        {
            sprite = s;
            this.message = message;
            this.e = e;
        }

        public override Sprite getSprite()
        {
            return sprite;
        }

        public override bool interact(Player p)
        {
            e(p,this);
            if(message!="")
            {
                p.pushState(new TextBox(p.getState(), p, message));
            }
            return true;
        }

        public override void onStepOn(Player p)
        {}

        public override void tick()
        {}
    }
}
