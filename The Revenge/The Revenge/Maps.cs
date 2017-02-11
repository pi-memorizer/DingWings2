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
}
