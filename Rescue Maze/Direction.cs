using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class Direction
    {
        public const int Up = 0;
        public const int Right = 1;
        public const int Down = 2;
        public const int Left = 3;

        public int CurrentDirection;

        public Direction(int direction)
        {
            if (direction >= 0 && direction < 4)
                CurrentDirection = direction;
            else
                CurrentDirection = 0;
        }

        public void RotateRight()
        {
            CurrentDirection = (CurrentDirection < 3) ? CurrentDirection + 1 : 0;
        }

        public void RotateLeft()
        {
            CurrentDirection = (CurrentDirection > 0) ? CurrentDirection - 1 : 3;
        }
    }
}
