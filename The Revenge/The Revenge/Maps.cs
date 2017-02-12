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
                SoundSystem.play("door");
            } else
            {
                p.pushState(new TextBox(p.getState(), p, "This door is locked."));
            }
        }

        public static void scienceMachine(Player p, Entity e)
        {
            SoundSystem.play("error");
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
            entities.Add(new WorldItem(WorldState.tileSprites[(int)Block.HammerHandle], Player.Item.HammerHandle, "Hammer Handle", this, 8, 5));
            entities.Add(new WorldItem(WorldState.tileSprites[(int)Block.HammerHead], Player.Item.HammerHead, "Hammer Head", this, 20, 18));
        }

        public override void tick(Player p)
        {

        }
    }

    class Map2 : StaticWorld
    {
        public Map2(int id, int width, int height) : base(id, width, height)
        {
            init();
        }

        public Map2()
        {
            init();
        }

        void init()
        {
            int[] coords = new int[]
            {
                24,0,
                29,0,
                30,0,
                31,0,
                31,1,
                26,2,
                11,3,
                26,3,
                27,3,
                28,3,
                10,4,
                11,4,
                27,4,
                28,4,
                29,4,
                9,5,
                10,5,
                11,5,
                14,6,
                9,7,
                14,7,
                21,7,
                10,8,
                21,8,
                22,8,
                1,9,
                3,9,
                4,9,
                9,9,
                11,9,
                22,9,
                29,9,
                30,9,
                31,9,
                3,10,
                4,10,
                11,10,
                10,31,
                3,11,
                18,11,
                20,11,
                17,12,
                19,12,
                21,13,
                14,14,
                15,14,
                18,14,
                22,14,
                14,15,
                15,15,
                17,15,
                18,15,
                21,15,

                24,15,
                25,15,
                6,16,
                7,16,
                13,16,
                15,16,
                24,16,
                25,16,
                6,17,
                14,17,
                16,17,
                20,17,
                21,17,
                22,17,
                24,17,
                13,18,
                15,18,
                16,18,
                21,18,
                22,18,
                24,18,
                0,20,
                0,21,
                1,21,
                30,23,
                31,23,
                31,24,
                19,25,
                19,26,
                20,26,
                21,26,
                26,29,
                31,29,
                26,30,
                27,30,
                19,31,
                20,31,
                21,31,
                26,31,
                27,31,
                30,31,
                31,31
            };

            for (int i = 0; i < coords.Length; i += 2)
                entities.Add(new Pushable(WorldState.tileSprites[(int)Block.Box], this, coords[i], coords[i + 1]));
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
