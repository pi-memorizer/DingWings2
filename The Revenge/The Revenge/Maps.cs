using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSystem
{
    class Map0 : StaticWorld
    {
        public Map0(int id, int width, int height) : base(id,width,height)
        {
            init();
        }

        public Map0()
        {
            init();
        }

        void init()
        {
            entities.Add(new WorldItem(WorldState.tileSprites[(int)Block.DuctTape], Player.Item.DuctTape, "Duct Tape", this, 15, 14));
            entities.Add(new WorldItem(WorldState.tileSprites[(int)Block.KeyCard], Player.Item.KeyCard, "Keycard", this, 27, 1));
            entities.Add(new WorldItem(WorldState.tileSprites[(int)Block.Wrench], Player.Item.Wrench, "Wrench", this, 0, 30));
            entities.Add(new EventEntity(WorldState.tileSprites[0], "", door, this, 12, 7));
            entities.Add(new EventEntity(WorldState.tileSprites[0], "", door, this, 12, 8));
        }

        void door(Player p, Entity e)
        {
            if(p.items[(int)Player.Item.KeyCard])
            {
                p.world.editing = true;
                p.world.setBlockAt(e.x, e.y, 0);
                p.world.editing = false;
                e.forceRemove();
                p.pushState(new TextBox(p.getState(), p, "The keycard opened the door!"));
            } else
            {
                p.pushState(new TextBox(p.getState(), p, "This door is locked."));
            }
        }

        public override void tick(Player p)
        {
            
        }
    }

    class Map1 : StaticWorld
    {
        public Map1(int id, int width, int height) : base(id, width, height)
        {
            init();
        }

        public Map1()
        {
            init();
        }

        void init()
        {
            entities.Add(new WorldItem(WorldState.tileSprites[(int)Block.HammerHandle], Player.Item.HammerHandle, "Hammer Handle", this, 5, 0));
            entities.Add(new WorldItem(WorldState.tileSprites[(int)Block.HammerHead], Player.Item.HammerHead, "Hammer Head", this, 20, 18));
        }

        public override void tick(Player p)
        {

        }
    }

    class Map3 : StaticWorld
    {
        public Map3(int id, int width, int height) : base(id, width, height)
        {
            init();
        }

        public Map3()
        {
            init();
        }

        void init()
        {
            DoorEntity[] doors =
            {
                new DoorEntity(false,this,8,13),
                new DoorEntity(false,this,10,15),
                new DoorEntity(false,this,12,13),
                new DoorEntity(true,this,8,17),
                new DoorEntity(false,this,6,19),
                new DoorEntity(true,this,4,14),
                new DoorEntity(true,this,1,19),
                new DoorEntity(false,this,12,17),
                new DoorEntity(false,this,14,19),
                new DoorEntity(false,this,8,25),
                new DoorEntity(true,this,3,30),
                new DoorEntity(false,this,8,29),
                new DoorEntity(true,this,2,23),
                new DoorEntity(false,this,1,27),
                new DoorEntity(true,this,12,27),
                new DoorEntity(false,this,16,30),
                new DoorEntity(false,this,18,23),
                new DoorEntity(true,this,20,24),
                new DoorEntity(true,this,20,17),
                new DoorEntity(true,this,17,15),
                new DoorEntity(false,this,26,23),
                new DoorEntity(true,this,28,28),
                new DoorEntity(true,this,20,28),
            };
            for (int i = 0; i < doors.Length; i++)
                entities.Add(doors[i]);
            DoorLever[] levers =
            {
                new DoorLever(new DoorEntity [] {doors[2-1],doors[4-1],doors[8 - 1],/*doors[18 - 1]*/ },this,10,13),
                new DoorLever(new DoorEntity [] {doors[1 - 1],doors[3 - 1],doors[9 - 1],doors[12 - 1],doors[15 - 1],doors[16 - 1] },this,14,15),
                new DoorLever(new DoorEntity [] {doors[10 - 1],doors[11 - 1],doors[23 - 1] },this,11,25),
                new DoorLever(new DoorEntity [] {doors[5 - 1],doors[11 - 1],doors[14 - 1],doors[22 - 1] },this,5,28),
                new DoorLever(new DoorEntity [] {doors[16 - 1] },this,1,16),
                new DoorLever(new DoorEntity [] {doors[17 - 1],doors[18 - 1],doors[21 - 1],doors[23 - 1] },this,23,30),
                new DoorLever(new DoorEntity [] {doors[20 - 1],doors[22 - 1] },this,22,21),
            };
            for(int i = 0; i < levers.Length; i++)
            {
                entities.Add(levers[i]);
            }
        }

        public override void tick(Player p)
        {

        }
    }
}
