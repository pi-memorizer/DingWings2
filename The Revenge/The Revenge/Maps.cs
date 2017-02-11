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
            entities.Add(new MessageEntity(WorldState.tileSprites[(int)Block.BallCactus], "Hello there! I am cactus! :)", this, 0, 0));
            entities.Add(new Pushable(WorldState.tileSprites[(int)Block.BasicDoor], this, 6, 6));
            entities.Add(new EventEntity(WorldState.tileSprites[(int)Block.DeadTree], "nope", nope, this, 0, 5));
        }

        void nope(Player p)
        {
            p.x = 16;
            p.y = 16;
        }

        public override void tick(Player p)
        {
            
        }
    }
}
