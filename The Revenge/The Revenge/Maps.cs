using System;
using System.Collections.Generic;
using System.Drawing;
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
            entities.Add(new EventEntity(WorldState.tileSprites[27], "", note, this, 16, 18));
            entities.Add(new EventEntity(WorldState.tileSprites[8], "The valve seems to be stuck. Maybe there's a maintenence closet around here somewhere?", mainValve, this, 20, 6));
        }

        void mainValve(Player p, Entity e)
        {
            EventEntity e2 = e as EventEntity;
            if (e2 == null) return;
            if(e2.message[0]=='T')
            {
                if (Player.items[(int)Player.Item.Wrench])
                {
                    Game.waterRising = true;
                    //WorldState.drainFrames += 3 * WorldState.WATER_SPEED / 2;
                    WorldState.waterLevel = 2*WorldState.WATER_SPEED;
                    e2.message = "After tightening the valve, you hear a rush of water below.";
                }
                else Console.WriteLine("no");
            }
        }

        void note(Player p, Entity e)
        {
            p.pushState(new TextBox(p.getState(), p, "This note will self destruct in approximately 4 to 6 weeks"));
            p.pushState(new TextBox(p.getState(), p, "Agent 842, because you missed the debriefing, we had Peter Warren drop off this note of instruction for you. What we need you to retrieve is in the vault on the lowest level. All the lower levels are flooded, so you'll need to find a way to deal with that. The main pipe for the building might be a good place to start. If you require Agent 794 to assist you, press any Player 2 key. Good luck."));
            e.forceRemove();
        }

        void door(Player p, Entity e)
        {
            if(Player.items[(int)Player.Item.KeyCard])
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
            int[] cells =
            {
                15,27,
                19,27,
                23,27,
                27,27,
                31,27,
            };
            for (int i = 0; i < cells.Length; i += 2)
                entities.Add(new MessageEntity(WorldState.tileSprites[0], "It's a small cell. The door seems to be welded shut.",this,cells[i],cells[i+1]));
            entities.Add(new MessageEntity(WorldState.tileSprites[0], "It's an expensive looking computer. You wonder if it still has Minesweeper.", this, 6, 22));
            entities.Add(new MessageEntity(WorldState.tileSprites[0], "Someone has changed the screensaver to flash 'Mike is dum.' You didn't think this high tech of a computer could run Windows 97.", this, 26, 0));
            int[] bigScience1 =
            {
                11,0,
                12,0,
                13,0,
                19,0,
                20,0,
                21,0,
                13,3,
                14,3,
                15,3,
                21,3,
                22,3,
                23,3,
                2,14,
                3,14,
                4,14,
                5,14,
                6,14
            };
            for(int i = 0; i < bigScience1.Length; i+=2)
            {
                entities.Add(new MessageEntity(WorldState.tileSprites[0], "You press the big red button. The machine beeps angrily and ignores your command.", this, bigScience1[i], bigScience1[i + 1]));
            }
            int[] bigScience2 =
            {
                15,0,
                16,0,
                17,0,
                23,0,
                24,0,
                25,0,
                17,3,
                18,3,
                19,3,
                2,11,
                3,11,
                4,11,
                5,11,
                6,11,
                2,17,
                3,17,
                4,17,
                5,17,
                6,17,
            };
            for (int i = 0; i < bigScience2.Length; i += 2)
            {
                entities.Add(new MessageEntity(WorldState.tileSprites[0], "You press the big red button. Nothing happens... you think?", this, bigScience2[i], bigScience2[i + 1]));
            }
            int[] smallScience1 =
            {
                27,21,
                7,11,
                3,14,
                17,18,
                17,12,
                23,13,
                20,14,
                23,17
            };
            for (int i = 0; i < smallScience1.Length; i += 2)
            {
                entities.Add(new MessageEntity(WorldState.tileSprites[0], "You try to adjust one of the sliders. On the screen appears 'no'.", this, smallScience1[i], smallScience1[i + 1]));
            }
            int[] smallScience2 =
            {
                13,21,
                17,15,
                20,12,
                27,19,
                20,17,
                23,16,
            };
            for (int i = 0; i < smallScience2.Length; i += 2)
            {
                entities.Add(new MessageEntity(WorldState.tileSprites[0], "As you reach for one of the knobs, the screen flashes with 'Don't even think about it'.", this, smallScience2[i], smallScience2[i + 1]));
            }
            entities.Add(new MessageEntity(WorldState.tileSprites[0], "It says 'Nick has told his dad joke for the day.' You don't know what that means.", this, 8, 11));
            entities.Add(new EventEntity(WorldState.tileSprites[0], "", boardedDoor, this, 28, 5));
            entities.Add(new EventEntity(WorldState.tileSprites[0], "", boardedDoor, this, 28, 6));
            entities.Add(new EventEntity(WorldState.tileSprites[8], "The twist knob for the valve seems to have fallen through a hole in the floor.", mainValve, this, 20, 7));
        }

        void boardedDoor(Player p, Entity e)
        {
            if (Player.items[(int)Player.Item.HammerHandle] && Player.items[(int)Player.Item.HammerHead])
            {
                if (Player.items[(int)Player.Item.DuctTape])
                {
                    p.world.editing = true;
                    p.world.setBlockAt(e.x, e.y, 0);
                    p.world.editing = false;
                    e.forceRemove();
                    p.pushState(new TextBox(p.getState(), p, "Somehow you fix a hammer and use it to pry open the door. Not even you are sure how you managed that."));
                    SoundSystem.play("door");
                } else
                {
                    p.pushState(new TextBox(p.getState(), p, "You have the hammer parts, but nothing to keep them together."));
                }
            }
            else
            {
                p.pushState(new TextBox(p.getState(), p, "The door is boarded shut. Maybe you can find something to pry the boards off?"));
            }
        }

        void mainValve(Player p, Entity e)
        {
            EventEntity e2 = e as EventEntity;
            if (e2 == null) return;
            if (e2.message!="")
            {
                if (Player.items[(int)Player.Item.Knob])
                {
                    p.pushState(new TextBox(p.getState(), p, "You reattach the knob and tighten it until the water recedes."));
                    WorldState.waterLevel = WorldState.WATER_SPEED * 3;
                    e2.message = "";
                }
            } 
        }

        public override void tick(Player p)
        {

        }
    }

    class Map2 : StaticWorld
    {
        public static int passId = Game.rand.Next(3);

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
            passId = Game.rand.Next(3);
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
            entities.Add(new WorldItem(WorldState.tileSprites[6], Player.Item.Knob, "Knob for the main pipe upstairs", this, 22, 7));
            entities.Add(new ValveEntity(WorldState.tileSprites[44], 20 * 60, this, 28, 8));
            entities.Add(new ValveEntity(WorldState.tileSprites[44], 20 * 60, this, 2, 8));
            entities.Add(new ValveEntity(WorldState.tileSprites[44], 20 * 60, this, 17, 19));
            string[] choices =
            {
                "what's on the wall in the southeastern corner room.",
                "the number of machines against the back wall in the biggest storage room.",
                "on a note taped to one of the boxes in Leroy's office."
            };
            entities.Add(new MessageEntity(WorldState.tileSprites[0], "Jim, I can't believe you forgot the password again! Remember it's " + choices[passId], this, 1, 16));
            entities.Add(new MessageEntity(WorldState.tileSprites[10], "It's a tiny motivational poster with a sticky note stuck to it proclaiming 'Dr. Jenkin's office.'", this, 27, 5));
            entities.Add(new MessageEntity(WorldState.tileSprites[26], "'Poster'", this, 27, 25));
            entities.Add(new EventEntity(WorldState.tileSprites[0], "", computer, this, 12, 1));
            entities.Add(new EventEntity(WorldState.tileSprites[0], "", computer, this, 12, 2));
            entities.Add(new EventEntity(WorldState.tileSprites[8], "", mainValve, this, 18, 7));
            entities.Add(new MessageEntity(WorldState.tileSprites[27], "password1", this, 27, 0));
        }

        void computer(Player p, Entity e)
        {
            p.pushState(new PasswordState(p.getState(), p));
        }

        void mainValve(Player p, Entity e)
        {
            EventEntity e2 = e as EventEntity;
            if (e2 == null) return;
            if (e2.message == "")
            {
                e2.message = "You tighten the valve and enjoy the glorious sound of draining water..";
                WorldState.waterLevel = WorldState.WATER_SPEED * 4;
            } else
            {
                e2.message = "Well now that's a useless valve.";
            }
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
                new DoorEntity(false,this,8,13), //1
                new DoorEntity(false,this,10,15),
                new DoorEntity(false,this,12,13),
                new DoorEntity(false,this,8,17),
                new DoorEntity(false,this,6,19), //5
                new DoorEntity(false,this,4,14),
                new DoorEntity(false,this,1,19),
                new DoorEntity(false,this,12,17),
                new DoorEntity(false,this,14,19),
                new DoorEntity(false,this,8,25),//10
                new DoorEntity(false,this,3,30),
                new DoorEntity(false,this,8,29),
                new DoorEntity(false,this,2,23),
                new DoorEntity(false,this,1,27),
                new DoorEntity(false,this,12,27),//15
                new DoorEntity(false,this,16,30),
                new DoorEntity(false,this,18,23),
                new DoorEntity(false,this,20,24),
                new DoorEntity(false,this,20,17),
                new DoorEntity(false,this,17,15),//20
                new DoorEntity(false,this,26,23),
                new DoorEntity(false,this,28,28),
                new DoorEntity(false,this,20,28),
            };
            for (int i = 0; i < doors.Length; i++)
                entities.Add(doors[i]);
            DoorLever[] levers =
            {
                new DoorLever(new DoorEntity [] {doors[1-1],doors[3-1],doors[4 - 1] },this,10,13),
                new DoorLever(new DoorEntity [] {doors[1 - 1],doors[6 - 1],doors[8 - 1],doors[9 - 1],doors[12 - 1],doors[16 - 1] },this,14,15),
                new DoorLever(new DoorEntity [] {doors[9- 1],doors[10 - 1],doors[14 - 1] },this,11,25),
                new DoorLever(new DoorEntity [] {doors[10 - 1],doors[11 - 1],doors[12 - 1]},this,5,28),
                new DoorLever(new DoorEntity [] {doors[7 - 1], doors[13-1],doors[16-1],doors[18-1] },this,1,16),
                new DoorLever(new DoorEntity [] {doors[17 - 1],doors[18 - 1],doors[19 - 1],doors[23 - 1] },this,23,30),
                new DoorLever(new DoorEntity [] {doors[20 - 1],doors[21-1],doors[22 - 1] },this,22,21),
            };
            for(int i = 0; i < levers.Length; i++)
            {
                entities.Add(levers[i]);
            }
            entities.Add(new EventEntity(WorldState.tileSprites[8], "", lastValve, this, 20, 7));
            entities.Add(new ValveEntity(WorldState.tileSprites[44], 20 * 60, this, 3, 19));
            entities.Add(new ValveEntity(WorldState.tileSprites[44], 20 * 60, this, 5, 23));
            entities.Add(new ValveEntity(WorldState.tileSprites[25], 20 * 60, this, 15, 16));
            entities.Add(new ValveEntity(WorldState.tileSprites[25], 20 * 60, this, 23, 20));
            entities.Add(new EventEntity(WorldState.tileSprites[0], "", lastDoor, this, 14, 8));
        }

        void lastValve(Player p, Entity e)
        {
            EventEntity e2 = e as EventEntity;
            if (e2 == null) return;
            if(e2.message=="")
            {
                e2.message = "The water seems to have permanently drained away. You only just barely resist to urge to cheer.";
                WorldState.waterLevel += 2000000000;
            } else
            {
                e2.message = "No more need for water pumping now.";
            }
        }

        void lastDoor(Player p, Entity e)
        {
            EventEntity e2 = e as EventEntity;
            if (e2 == null) return;
            if(e2.message=="")
            {
                p.world.editing = true;
                p.world.setBlockAt(16, 7, 0);
                p.world.setBlockAt(17, 7, 0);
                p.world.editing = false;
                p.pushState(new TextBox(p.getState(), p, "Ka'ch! You just sort of felt the need to make the sound effect."));
            } else
            {
                p.pushState(new TextBox(p.getState(), p, "You proceed to play Minesweeper. It's not very fun."));
            }
        }

        public override void tick(Player p)
        {
            if (p.y == 6 && (p.x == 16 || p.x == 17)&&!(p.getState() is EndState)) p.setState(new EndAnimation(p));
        }
    }

    class EndAnimation : WorldState
    {
        MurderBunny buns;
        int frames = 0;
        Sprite tbc = new Sprite("tbc.png");

        public EndAnimation(Player player) : base(player)
        {
            buns = new MurderBunny(p.world, 17, 3);
            p.world.entities.Add(buns);
        }

        static SolidBrush sb = new SolidBrush(Color.FromArgb(128, Color.RosyBrown));
        public override void draw(Graphics g, Player p)
        {
            base.draw(g, p);
            if(frames>=140)
            {
                if (frames >= 170)
                    tbc.draw(g, 60, 128);
                else
                    tbc.draw(g, 160 - 5*(frames - 150), 128);

                g.FillRectangle(sb,0,0,160,144);
            }
        }

        public override void run(Player p)
        {
            p.world.editing = true;
            p.world.setBlockAt(17, 3, 0);
            p.world.editing = false;

            if (p.xOffset == 0 && p.x == 16)
            {
                p.dir = 1;
                if(p.y==3&&p.yOffset==0)
                {
                    p.dir = 0;
                    if(frames>=270)
                    {
                        WorldState.worldinit();
                        Player[] players = Game.players;
                        for (int i = 0; i < players.Length; i++)
                        {
                            players[i] = new Player(i + 1);
                            players[i].setState(new EndState(p));
                        }
                        Game.waterRising = false;
                    } else
                    if(frames>=150)
                    {
                        frames++;
                    } else if(frames>=140)
                    {
                        buns.attack = true;
                        frames++;
                    } else if(frames==0)
                    {
                        SoundSystem.setBackgroundMusic("tbc");
                        frames++;
                    } else
                    {
                        frames++;
                    }
                } else if(p.yOffset==0)
                {
                    p.y--;
                    p.yOffset = 15;
                    p.dir = 1;
                } else
                {
                    p.yOffset--;
                    p.dir = 1;
                }
            }
            else if (p.x == 17)
            {
                p.x = 16;
                p.xOffset = 15;
                p.dir = 2;
            }
            else p.xOffset--;
        }
    }

    class MurderBunny : Entity
    {
        public MurderBunny(World w, int x, int y) : base(w,x,y)
        {

        }

        public bool attack = false;
        Sprite attackSprite = WorldState.tileSprites[279], normalSprite = WorldState.tileSprites[127];
        public override Sprite getSprite()
        {
            if (attack)
                return attackSprite;
            else
                return normalSprite;
        }

        public override bool interact(Player p)
        {
            return false;
        }

        public override void onStepOn(Player p)
        {
        }

        public override void tick()
        {
        }
    }
}
