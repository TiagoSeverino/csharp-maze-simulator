using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class RobotTile
    {
        public int tileType;
        public int rightWall;
        public int bottomWall;

        public RobotTile()
        {
            tileType = TileType.Void;
            rightWall = Wall.Unknown;
            bottomWall = Wall.Unknown;
        }
    }
}
