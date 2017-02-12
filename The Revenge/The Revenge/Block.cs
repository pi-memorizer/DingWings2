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
        Air = 0, CreamWall, DuctTape, RabbitBed, HammerHead = 4, LabFloor, Valve, TrashCan = 11,
        OldDoorRight = 16, BoringCarpet, KeyCard, Wrench, HammerHandle, LittleScienceFloor, Box,
        OldDoorLeft = 32, DeskSingle, DeskLeft, DeskRight, LeverDown = 37,
        OldDoorDown = 48, TableTopLeft, TableTopMiddle, TableTopRight, TableTopExtendsRight, LeverUp,
        OldDoorUp = 64, TableLeftMiddle, TableMiddle, TableRightMiddle, TableMiddleColumn, TableTopColumn,
        DownStairs = 80, TableBottomLeft, TableBottomMiddle, TableBottomRight, TableBottomExtendsRight, TableBottomColumn,
        UpStairs = 96, TableLeftRow, TableMiddleRow, TableRightRow, TableSingle,
        ChairLeft = 256, ChairDown, ChairRight, ChairUp, ComputerOn, ComputerOff, ComputerDesk, ComputerBack, BigScience, LittleScience,
        DoorRight = 272, DoorLeft, DoorSide, PottedPlant = 278,
        Floor1Wall1 = 512, Floor1Wall2, Floor1Wall3, Floor1Wall4, Floor1Wall5, Floor1Wall6, Floor1Wall7, Floor1Wall8, Floor1Wall9, Floor1Wall10, Floor1Wall11, Floor1Wall12,
    }

    public class BlockData
    {
        static bool[] values = new bool[WorldState.NUM_TILES];

        static BlockData()
        {
            for (int i = 0; i < 256; i++)
                setTrue((Block)i);
            int[] b =
            {
                49,50,51,52,53,37,60,61,
                65,66,67,68,69,64+16+7
            };
            for (int i = 0; i < b.Length; i++)
                setFalse((Block)b[i]);
            for(int i = 256; i < 512; i++)
            {
                setTrue((Block)i);
            }
            setFalse(Block.DoorSide);
            setFalse(Block.DoorSide + 16);
            for(int i = 512; i < 512+52; i++)
            {
                int x = (i-512) % 13;
                if (x <= 4) setTrue((Block)i);
                else if (x == 12 || x == 6 || x == 7) setTrue((Block)i);
                else setFalse((Block)i);
            }
            for(int i = 0; i < 4; i++)
            {
                setTrue((Block)(512 + 13 * 4 + 8 + i));
            }
        }

        static void setTrue(Block b)
        {
            values[(int)b] = true;
        }

        static void setFalse(Block b)
        {
            values[(int)b] = false;
        }

        public static bool canHaveWater(Block b)
        {
            return values[(int)b];
        }

        public static bool canHaveWater(int i)
        {
            return values[i];
        }
    }
}
