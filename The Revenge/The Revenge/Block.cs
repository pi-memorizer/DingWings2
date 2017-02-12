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
        Air = 0, CreamWall, DuctTape, RabbitBed, HammerHead = 4, LabFloor, Valve,
        OldDoorRight = 16, BoringCarpet, KeyCard, Wrench, HammerHandle, LittleScienceFloor,
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
            setTrue(Block.Air);
            setTrue(Block.ChairDown);
            setTrue(Block.ChairLeft);
            setTrue(Block.ChairRight);
            setTrue(Block.ChairUp);
            setTrue(Block.TableBottomColumn);
            setTrue(Block.TableBottomExtendsRight);
            setTrue(Block.TableBottomLeft);
            setTrue(Block.TableBottomMiddle);
            setTrue(Block.TableBottomRight);
            setTrue(Block.DeskLeft);
            setTrue(Block.DeskRight);
            setTrue(Block.DeskSingle);
            setTrue(Block.Floor1Wall1);
            setTrue(Block.Floor1Wall2);
            setTrue(Block.Floor1Wall3);
            setTrue(Block.Floor1Wall4);
            setTrue(Block.Floor1Wall7);
            setTrue(Block.Floor1Wall8);
        }

        static void setTrue(Block b)
        {
            values[(int)b] = true;
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
