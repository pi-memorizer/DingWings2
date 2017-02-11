using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GameSystem
{
    //enumerator for Blocks, mapped to integers and here for us to reference in code
    public enum Block : ushort
    {
        Air = 0, Grass, Water, DeepWater, OceanFloor, ForestFloor, DesertSand, Swamp,
        Tundra, Taiga, BeachGravel, BeachSand, MountainRock,
        FlyAlgaric = 16, Portabella, Puffball, InkCap, HenOfTheWoods, Morel, Enoki, Truffel,
        Daisy, Tulip, Lily, Violet, Rose, Heather, Petunia, Orchid,
        BallCactus, CowSkull, Coral,
        UpStairs, DownStairs,
        Tree = 256, Spruce, DeadTree, Larch, Cactus, Sunflower, Kelp, Mangrove,
        Cloud = 512, StreetCloud, BasicSiding, BasicDoor, BasicRoofing, BasicWindow
    }
}
