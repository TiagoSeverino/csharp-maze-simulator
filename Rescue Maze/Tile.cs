using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class Tile
    {
        public int RightWall;
        public int BottomWall;

        public int Type;
    }

    static class Wall
    {
        public const int Unknown = 0;
        public const int No = 1;
        public const int Yes = 2;
    }

    static class TileType
    {
        public const int Void = 0;
        public const int White = 1;
        public const int Black = 2;
    }
}
