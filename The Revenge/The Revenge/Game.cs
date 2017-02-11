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
        static bool waterRising = false;

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

        //called every frame
        public static void run()
        {
            Player[] p = players; //because lazy
            if (waterRising)
            {
                if (WorldState.drainFrames == 0)
                {
                    WorldState.waterLevel--;
                    if (WorldState.waterLevel == 0)
                    {
                        //TODO do game lose
                        WorldState.waterLevel = 2 * WorldState.WATER_SPEED;
                    }
                    else if (WorldState.waterLevel % WorldState.WATER_SPEED == 0)
                    {
                        int level = WorldState.waterLevel / WorldState.WATER_SPEED;
                        WorldState.pipeUsesLeft[level]--;
                        if (WorldState.pipeUsesLeft[level] == 0)
                        {
                            //TODO do game lose
                        }
                    }
                }
                else
                {
                    WorldState.waterLevel--;
                    WorldState.drainFrames--;
                }
            } else
            {
                for (int i = 0; i < p.Length; i++)
                    if (p[i].level == 1) waterRising = true;
            }
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
