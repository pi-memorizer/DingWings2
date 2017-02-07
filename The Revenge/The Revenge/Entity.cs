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
        public int x, y; //coordinates
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
        public abstract bool getCanStepOn(Player p); //if the player can step on it or if it impedes motion
        public abstract void onStepOn(Player p); //called when the player steps on it (i.e. land mines)
        public virtual bool needsRemoval() { return remove; } //called to check if the entity needs removal
        public abstract bool interact(Player p); //called if the player selects the entity, true if interaction was made
    }
}
