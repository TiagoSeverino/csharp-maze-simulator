using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class NextVoidTiles
    {
        public bool Left, Right, Up, Down;
        public int VoidTiles = 0;
        public NextVoidTiles(bool left, bool right, bool up, bool down)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;

            if (Left)
                VoidTiles++;

            if (Right)
                VoidTiles++;

            if (Up)
                VoidTiles++;

            if (Down)
                VoidTiles++;

        }
    }
}
