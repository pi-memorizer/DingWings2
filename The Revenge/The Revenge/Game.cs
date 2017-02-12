using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GameSystem
{
    class Game
    {
        public static Player[] players = new Player[0]; //array of all players in the game
        public static int KEY_DELAY = 10; //for things that have timed keys, how fast they can respond
        public static Random rand = new Random(); //global random
        public static long frames = 0; //what frame the game is in, useful for animated sprites
        public static string[] args;
        public static bool waterRising = false;

        static Game()
        {
            SoundSystem.load("beige", "wav");
            SoundSystem.setLoop("beige");
            SoundSystem.load("emergency", "wav");
            SoundSystem.setLoop("emergency");
            SoundSystem.load("emergency2", "wav");
            SoundSystem.setLoop("emergency2");
            SoundSystem.load("ohShiitakeMushrooms", "wav");
            SoundSystem.setLoop("ohShiitakeMushrooms");
            SoundSystem.load("creepyMusic", "wav");
            SoundSystem.setLoop("creepyMusic");
            SoundSystem.load("dracula", "wav");
            SoundSystem.setLoop("dracula");
            SoundSystem.load("main", "wav");
            SoundSystem.setLoop("main");
            SoundSystem.load("eeyore", "wav");
            SoundSystem.setLoop("eeyore");
            SoundSystem.load("box move", "wav");
            SoundSystem.load("door", "wav");
            SoundSystem.load("error", "wav");
            SoundSystem.load("footsteps","wav");
            SoundSystem.setVolume("footsteps", .3);
            SoundSystem.load("lever", "wav");
            SoundSystem.load("stairs", "wav");
            SoundSystem.load("water", "wav");
            SoundSystem.setVolume("water", .3);
        }

        public static Player getPlayer(int id)
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i].id == id)
                    return players[i];
            return null;
        }

        public static void addPlayer(int id)
        {
            Player[] newPlayers = new Player[players.Length + 1];
            for(int i = 0; i < players.Length; i++)
            {
                newPlayers[i] = players[i];
            }
            newPlayers[players.Length] = new Player(id);
            players = newPlayers;
        }

        public static void removePlayer(int id)
        {
            Player[] newPlayers = new Player[players.Length - 1];
            for (int i = 0, j = 0; i < players.Length; i++)
            {
                if(players[i].id!=id)
                {
                    newPlayers[j] = players[i];
                    j++;
                }
            }
            players = newPlayers;
        }

        public static void lose(string [] reasoning)
        {
            WorldState.worldinit();
            for(int i = 0; i < players.Length; i++)
            {
                players[i] = new Player(i+1);
                players[i].setState(new DeathState(players[i], reasoning[i%reasoning.Length]));
            }
            waterRising = false;
        }

        //called every frame
        public static void run()
        {
            Player[] p = players; //because laz
            for (int i = 0; i < p.Length; i++)
            {
                if(p[i].getState() is WorldState)
                {
                    WorldState w = (WorldState)p[i].getState();
                    if (w.getWaterPixel()==16)
                        w.WALK_SPEED = 1;
                    else
                        w.WALK_SPEED = 2;
                }
                if(p[i].isScuba)
                {
                    if(p[i].breath==0)
                    {
                        if(i==0)
                            lose(new string[] { "Humans aren't built for living underwater.", "The other player forgot how to breath."});
                        else
                            lose(new string[] { "The other player forgot how to breath.", "Humans aren't built for living underwater." });
                    } else
                    {
                        p[i].breath--;
                    }
                } else
                {
                    if (p[i].breath < Player.MAX_BREATH)
                        p[i].breath++;
                }
                p[i].run();
            }
            bool canTick = false; //whether or not any worlds can tick, i.e. if any players are outside of menus
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].getState() is WorldState && !(p[i].getState() is WorldEditor))
                {
                    canTick = true;
                    p[i].world.tick(p[i]);
                }
            }
            if (canTick)
            {
                if (waterRising)
                {
                    if (WorldState.drainFrames == 0)
                    {
                        WorldState.waterLevel--;
                        if (WorldState.waterLevel == 0)
                        {
                            lose(new string[] { "Inundation complete. Might want to stop that next time." });
                            WorldState.waterLevel = WorldState.WATER_SPEED;
                        }
                        else if (WorldState.waterLevel % WorldState.WATER_SPEED == 0)
                        {
                            int level = WorldState.waterLevel / WorldState.WATER_SPEED;
                            if (level >= 0 && level < 4)
                            {
                                WorldState.pipeUsesLeft[level]--;
                                if (WorldState.pipeUsesLeft[level] == 0)
                                {
                                    lose(new string[] { "There was no hope for floor " + (level + 1) + "'s pipe any longer." });
                                }
                            }
                        }
                    }
                    else
                    {
                        WorldState.waterLevel++;
                        WorldState.drainFrames--;
                    }
                }
                frames++;
                List<World> tickedWorlds = new List<World>(); //worlds in this list have already ticked, needed so that they don't double up
                for (int i = 0; i < p.Length; i++)
                {
                    if (!tickedWorlds.Contains(p[i].world))
                    {
                        World w = p[i].world;
                        for (int j = 0; j < w.entities.Count; j++)
                        {
                            Entity e = w.entities[j];
                            e.tick();
                            if (e.needsRemoval())
                            {
                                w.entities.Remove(e);
                                j--;
                            }
                        }
                        tickedWorlds.Add(w);
                    }
                }
            }
        }
    }
}
