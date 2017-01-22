using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Rescue_Maze
{
    class Maze
    {
        Robot robot;
        Random rnd;

        public Maze(int ArenaWidth, int ArenaHeight)
        {
            rnd = new Random();
            Arena.NewArena(ArenaWidth, ArenaHeight);
            robot = new Robot(ArenaWidth, ArenaHeight);
        }

        public void Start()
        {
            new Thread(MapManager).Start();
            new Thread(RobotManager).Start();
        }

        void MapManager()
        {
            while (true)
            {
                Console.Clear();
                Arena.ShowMap();
                Thread.Sleep(250);
            }
        }

        void RobotManager()
        {
            robot.Start();

            while (true)
            {
                robot.Step();
                Thread.Sleep(250);
            }
        }
    }
}
