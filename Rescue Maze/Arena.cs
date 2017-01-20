using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    static class Arena
    {
        static Random rnd = new Random();
        static Tile[,] map;
        static int width, height;
        static int RobotX, RobotY;
        static Direction RobotDirection;
        static int StartX, StartY;

        #region Map Generator

        public static void NewArena(int Width, int Height)
        {
            width = Width;
            height = Height;

            map = new Tile[width, height];

            RandomRobotCoordenates();

            InitializeTiles();
            GenerateTiles();
            CloseWalls();
        }

        static void RandomRobotCoordenates()
        {
            RobotDirection = new Direction(Direction.Up);
            StartX = rnd.Next(1, width - 1);
            StartY = rnd.Next(1, height - 1);
            RobotX = StartX;
            RobotY = StartY;
        }

        static void InitializeTiles()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    map[x, y] = new Tile();
        }

        static void GenerateTiles()
        {
            foreach (Tile tile in map)
            {
                tile.BottomWall = (rnd.Next(0, 10) < 7) ? Wall.No : Wall.Yes;
                tile.RightWall = (rnd.Next(0, 10) < 7) ? Wall.No : Wall.Yes;

                int random = rnd.Next(0, 100);

                int tileType;

                if (random < 96)
                    tileType = TileType.White;
                else
                    tileType = TileType.Black;

                tile.Type = tileType;
            }
        }

        static void CloseWalls()
        {
            //Close Left
            for (int y = 0; y<height; y++)
                map[0, y].RightWall = Wall.Yes;

            //Close Right
            for (int y = 0; y<height; y++)
                map[width - 1, y].RightWall = Wall.Yes;

            //Close Top
            for (int x = 0; x<width; x++)
                map[x, 0].BottomWall = Wall.Yes;

            //Close Bottom
            for (int x = 0; x<width; x++)
                map[x, height - 1].BottomWall = Wall.Yes;
        }

        #endregion

        #region Output

        public static void ShowMap()
        {
            ShowTiles();
        }

        static void ShowTiles()
        {
            Console.WriteLine("Start Position: {0}, {1}. Position: {2}, {3}", StartX, StartY, RobotX, RobotY);
            for (int y = 0; y < height; y++)
            {
                string line = String.Empty;
                string line2 = String.Empty;
                for (int x = 0; x < width; x++)
                {
                    string tileType;

                    switch (map[x, y].Type)
                    {
                        case TileType.Black:
                            tileType = "░";
                            break;
                        default:
                            tileType = " ";
                            break;
                    }

                    if (x == RobotX && y == RobotY)
                    {
                        switch (RobotDirection.CurrentDirection)
                        {
                            case Direction.Up:
                                tileType = "^" + tileType;
                                break;
                            case Direction.Right:
                                tileType = ">" + tileType;
                                break;
                            case Direction.Left:
                                tileType = "<" + tileType;
                                break;
                            default:
                                tileType = "!" + tileType;
                                break;
                        }
                        tileType = "■" + tileType;
                    }
                    else
                        tileType += tileType + tileType;
                    line += (tileType);

                    line2 += (map[x, y].BottomWall == Wall.Yes && x != 0) ? "___" : "   ";

                    if (map[x, y].RightWall == Wall.Yes && y != 0)
                    {
                        line += "║";
                        line2 += "║";
                    }
                    else
                    {
                        line += " ";
                        if (map[x, y].BottomWall == Wall.Yes && x != 0)
                        {
                            line2 += "_";
                        }
                        else
                        {
                            line2 += " ";
                        }
                    }
                }
                if (y > 0)
                    Console.WriteLine(line.Substring(3));
                Console.WriteLine(line2.Substring(3));
            }
            Console.WriteLine();
        }

        #endregion

        #region Robot Controller

        #region Robot Functions

        public static bool MoveTile()
        {
            switch (RobotDirection.CurrentDirection)
            {
                case Direction.Up:
                    return MoveUp();
                case Direction.Right:
                    return MoveRight();
                case Direction.Down:
                    return MoveDown();
                default:
                    return MoveLeft();
            }
        }

        public static Sonar GetTiles()
        {
            switch (RobotDirection.CurrentDirection)
            {
                case Direction.Up:
                    return TilesUp();
                case Direction.Right:
                    return TilesRight();
                case Direction.Down:
                    return TilesDown();
                default:
                    return TilesLeft();
            }
        }

        public static int GetFloor()
        {
            return map[RobotX, RobotY].Type;
        }

        public static void RotateLeft()
        {
            RobotDirection.RotateLeft();
        }

        public static void RotateRight()
        {
            RobotDirection.RotateRight();
        }

        #endregion

        #region Get Tiles

        static Sonar TilesUp()
        {
            return new Sonar(IsTileLeft(), IsTileUp(), IsTileRight());
        }

        static Sonar TilesRight()
        {
            return new Sonar(IsTileUp(), IsTileRight(), IsTileDown());
        }

        static Sonar TilesDown()
        {
            return new Sonar(IsTileRight(), IsTileDown(), IsTileLeft());
        }

        static Sonar TilesLeft()
        {
            return new Sonar(IsTileDown(), IsTileLeft(), IsTileUp());
        }

        static bool IsTileLeft()
        {
            return (map[RobotX - 1, RobotY].RightWall == Wall.Yes);
        }

        static bool IsTileRight()
        {
            return (map[RobotX, RobotY].RightWall == Wall.Yes);
        }

        static bool IsTileUp()
        {
            if (RobotY == 0)
                return false;
            return (map[RobotX, RobotY-1].BottomWall == Wall.Yes);
        }

        static bool IsTileDown()
        {
            return (map[RobotX, RobotY].BottomWall == Wall.Yes);
        }

        #endregion

        #region Movement

        static bool MoveUp()
        {
            if (RobotY > 0)
            {
                RobotY--;
                return true;
            }
            return false;
        }

        static bool MoveRight()
        {
            if (RobotX < width - 1)
            {
                RobotX++;
                return true;
            }
            return false;
        }

        static bool MoveDown()
        {
            if (RobotY < height - 1)
            {
                RobotY++;
                return true;
            }
            return false;
        }

        static bool MoveLeft()
        {
            if (RobotX > 1)
            {
                RobotX--;
                return true;
            }
            return false;
        }

        #endregion

        #endregion
    }
}
