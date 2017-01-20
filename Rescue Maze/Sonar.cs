using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class Sonar
    {
        public bool Front;
        public bool Left;
        public bool Right;

        public Sonar(bool left, bool front, bool right)
        {
            Front = front;
            Left = left;
            Right = right;
        }
    }
}
