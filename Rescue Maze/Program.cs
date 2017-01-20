using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RoboCup Junior Maze Simulator";
            Maze maze = new Maze(42, 19);//42, 19 Max console size
            maze.Start();
        }
    }
}
