﻿using System;
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
                    WorldState w = p.getState() as WorldState;
                    if(w==null)
                    {
                        return false;
                    }
                    if (w.getWaterPixel() > 0)
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
                        SoundSystem.play("box move");
                    } else
                    {
                        p.pushState(new TextBox(w, p, "A large box. You wonder if it would float."));
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
            Player.items[(int)item] = true;
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
        public string message;
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

    class DoorLever : Entity
    {
        DoorEntity[] coords;
        static Sprite leverUp = WorldState.tileSprites[(int)Block.LeverUp], leverDown = WorldState.tileSprites[(int)Block.LeverDown];
        bool toggled = false;

        public DoorLever(DoorEntity[] coords, World w, int x, int y) : base(w,x,y)
        {
            this.coords = coords;
        }

        public override Sprite getSprite()
        {
            return toggled ? leverUp : leverDown;
        }

        public override bool interact(Player p)
        {
            toggled = !toggled;
            SoundSystem.play("lever");
            for(int i = 0; i < coords.Length; i++)
            {
                DoorEntity d = p.world.entityAt(coords[i].x, coords[i].y) as DoorEntity;
                if(d!=null)
                {
                    d.open = !d.open;
                }
            }
            return true;
        }

        public override void onStepOn(Player p)
        {}

        public override void tick()
        {}
    }

    class DoorEntity : Entity
    {
        public bool open;
        public Sprite sprite = null;

        public DoorEntity(bool open, World w, int x, int y) : base(w,x,y)
        {
            this.open = open;
        }

        public override Sprite getSprite()
        {
            if(sprite==null)
            {
                if (world.getBlockAt(x, y - 1) == 0)
                {
                    sprite = WorldState.tileSprites[(int)Block.DoorLeft+16];
                }
                else
                {
                    sprite = WorldState.tileSprites[(int)Block.DoorSide+16];
                }
            }
            return !open ? sprite : WorldState.tileSprites[0];
        }

        public override bool collides(int x, int y, int xOffset, int yOffset, int width, int height)
        {
            if (!open) return base.collides(x, y, xOffset, yOffset, width, height); else return false;
        }

        public override bool interact(Player p)
        {
            p.pushState(new TextBox(p.getState(), p, "The door doesn't want to budge."));
            return true;
        }

        public override void onStepOn(Player p)
        { }

        public override void tick()
        {}
    }

    class ValveEntity : Entity
    {
        bool active = true;
        int amount;
        Sprite sprite;

        public ValveEntity(Sprite s, int amount, World w, int x, int y) : base(w,x,y)
        {
            sprite = s;
            this.amount = amount;
        }

        public override Sprite getSprite()
        {
            return sprite;
        }

        public override bool interact(Player p)
        {
            if(active)
            {
                active = false;
                WorldState.drainFrames += amount;
                p.pushState(new TextBox(p.getState(), p, "You twisted the valve and water slowly recedes!"));

            } else
            {
                p.pushState(new TextBox(p.getState(), p, "The valve is doing all it can!"));
            }
            return true;
        }

        public override void onStepOn(Player p)
        {}

        public override void tick()
        {}
    }

    class PasswordState : GameState
    {
        GameState caller;
        public PasswordState(GameState caller, Player p) : base(p)
        {
            this.caller = caller;
            flags = new int[11];
            for (int i = 0; i < 11; i++) flags[i] = -2;
        }

        public override void draw(Graphics g, Player p)
        {
            caller.draw(g, p);
        }

        public override void run(Player p)
        {
            if(Map2.passId==-1)
            {
                p.popState();
                p.pushState(new TextBox(p.getState(),p,"So much work for a password."));
                return;
            }
            if(flags[0]==-2)
            {
                p.pushState(new KeyboardDialogue(this, "Password:", 0, 10, p));
            } else if(flags[0]==-1)
            {
                p.popState();
            } else
            {
                string s = KeyboardDialogue.getString(flags, 1, flags[0]);
                string[] choices =
                {
                    "poster",
                    "11",
                    "password1"
                };
                if(s.ToLower()==choices[Map2.passId])
                {
                    Map2.passId = -1;
                    World w = p.world;
                    w.editing = true;
                    w.setBlockAt(12, 1, 0);
                    w.setBlockAt(12, 2, 0);
                    w.editing = false;
                    w.entityAt(12, 1).forceRemove();
                    w.entityAt(12, 2).forceRemove();
                    SoundSystem.play("door");
                }
                p.popState();
            }
        }
    }
}
