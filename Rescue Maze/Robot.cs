using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class Robot
    {
        RobotTile[,] map;
        int mapWidth;
        int mapHeight;
        Direction direction;
        int StartingX, StartingY, x, y;
        bool IsAutonomous = true;

        bool GoInitial = false;

        public Robot(int MapWidth, int MapHeight)
        {
            if (MapWidth > MapHeight)
            {
                mapWidth = (MapWidth + 2) * 2;
                mapHeight = (MapWidth + 2) * 2;
            }
            else
            {
                mapWidth = MapHeight * 2 + 2;
                mapHeight = MapHeight * 2 + 2;
            }
            
        }

        public void Start()
        {
            map = new RobotTile[mapWidth, mapHeight];

            for (int X = 0; X < mapWidth; X++)
                for (int Y = 0; Y < mapHeight; Y++)
                    map[X, Y] = new RobotTile();

            direction = new Direction(Direction.Up);

            StartingX = mapWidth / 2;
            StartingY = mapHeight / 2;

            x = StartingX;
            y = StartingY;

            GetTileInfo();
        }

        public void Step()
            {
            if (IsAutonomous)
                if (KnowsAllTiles())
                {
                    if (GoInitial)
                    {
                        ReturnInitialTile();

                        if(x == StartingX && y == StartingY)
                            IsAutonomous = false;
                        else
                            MoveTile();
                    }
                    else
                    {
                        if (NearVoidTile())
                            RotateNextTile();
                        else
                            FloodFillVoidTile();

                        MoveTile();
                        GetTileInfo();
                    }
                }
                else
                {
                    RotateToUnknownWall();
                    GetTileInfo();
                }
            else
                ManualMove();
        }

        void FloodFillVoidTile()
        {
            FloodFill floodFill = new FloodFill(mapWidth, mapHeight);
            floodFill.AddTile(x, y, 0, 0);

            bool TileFound = false;

            FloodTile lastTile = floodFill.GetFloodAt(0)[0];

            for (int i = 0; !TileFound; i++)
            {
                if(floodFill.GetFloodAt(i).Count > 0)
                {
                    foreach (FloodTile floodTile in floodFill.GetFloodAt(i))
                    {
                        if (map[floodTile.x, floodTile.y].tileType == TileType.Void)
                        {
                            TileFound = true;
                            lastTile = floodTile;
                            break;
                        }

                        bool Left, Right, Bottom, Top;

                        Left = (map[floodTile.x - 1, floodTile.y].rightWall == Wall.No && (floodFill.tile[floodTile.x - 1, floodTile.y] == null || floodFill.tile[floodTile.x - 1, floodTile.y].i > i));
                        Right = (map[floodTile.x, floodTile.y].rightWall == Wall.No && (floodFill.tile[floodTile.x + 1, floodTile.y] == null || floodFill.tile[floodTile.x + 1, floodTile.y].i > i));
                        Bottom = (map[floodTile.x, floodTile.y].bottomWall == Wall.No && (floodFill.tile[floodTile.x, floodTile.y + 1] == null || floodFill.tile[floodTile.x, floodTile.y + 1].i > i));
                        Top = (map[floodTile.x, floodTile.y - 1].bottomWall == Wall.No && (floodFill.tile[floodTile.x, floodTile.y - 1] == null || floodFill.tile[floodTile.x, floodTile.y - 1].i > i));

                        if (Left)
                            floodFill.AddTile(floodTile.x - 1, floodTile.y, i + 1, floodTile.id);

                        if (Right)
                            floodFill.AddTile(floodTile.x + 1, floodTile.y, i + 1, floodTile.id);

                        if (Bottom)
                            floodFill.AddTile(floodTile.x, floodTile.y + 1, i + 1, floodTile.id);

                        if (Top)
                            floodFill.AddTile(floodTile.x, floodTile.y - 1, i + 1, floodTile.id);

                        lastTile = floodTile;
                    }
                }
                else
                {
                    GoInitial = true;
                    return;
                }
            }

            while(lastTile.i > 1)
            {
                foreach (FloodTile floodTile in floodFill.GetFloodAt(lastTile.i - 1))
                {
                    if (floodTile.id == lastTile.parentID)
                    {
                        lastTile = floodTile;
                        break;
                    }
                }
            }

            if (lastTile.x < x)
            {
                if (direction.CurrentDirection == Direction.Right)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Up)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Down)
                    RotateRight();
                return;
            }

            if (lastTile.x > x)
            {
                if (direction.CurrentDirection == Direction.Left)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Up)
                    RotateRight();

                if (direction.CurrentDirection == Direction.Down)
                    RotateLeft();
                return;
            }

            if (lastTile.y < y)
            {
                if (direction.CurrentDirection == Direction.Down)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Left)
                    RotateRight();

                if (direction.CurrentDirection == Direction.Right)
                    RotateLeft();
                return;
            }

            if (lastTile.y > y)
            {
                if (direction.CurrentDirection == Direction.Up)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Left)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Right)
                    RotateRight();
                return;
            }
        }

        void ReturnInitialTile()
        {
            FloodFill floodFill = new FloodFill(mapWidth, mapHeight);
            floodFill.AddTile(x, y, 0, 0);

            bool TileFound = false;

            FloodTile lastTile = floodFill.GetFloodAt(0)[0];
            for(int i = 0; !TileFound; i++)
            {
                foreach (FloodTile floodTile in floodFill.GetFloodAt(i))
                {
                    if (floodTile.x == StartingX && floodTile.y == StartingY)
                    {
                        TileFound = true;
                        lastTile = floodTile;
                        break;
                    }

                    bool Left, Right, Bottom, Top;

                    Left = (map[floodTile.x - 1, floodTile.y].rightWall == Wall.No && (floodFill.tile[floodTile.x - 1, floodTile.y] == null || floodFill.tile[floodTile.x - 1, floodTile.y].i > i));
                    Right = (map[floodTile.x, floodTile.y].rightWall == Wall.No && (floodFill.tile[floodTile.x + 1, floodTile.y] == null || floodFill.tile[floodTile.x + 1, floodTile.y].i > i));
                    Bottom = (map[floodTile.x, floodTile.y].bottomWall == Wall.No && (floodFill.tile[floodTile.x, floodTile.y + 1] == null || floodFill.tile[floodTile.x, floodTile.y + 1].i > i));
                    Top = (map[floodTile.x, floodTile.y - 1].bottomWall == Wall.No && (floodFill.tile[floodTile.x, floodTile.y - 1] == null || floodFill.tile[floodTile.x, floodTile.y - 1].i > i));

                    if (Left)
                        floodFill.AddTile(floodTile.x - 1, floodTile.y, i + 1, floodTile.id);

                    if (Right)
                        floodFill.AddTile(floodTile.x + 1, floodTile.y, i + 1, floodTile.id);

                    if (Bottom)
                        floodFill.AddTile(floodTile.x, floodTile.y + 1, i + 1, floodTile.id);

                    if (Top)
                        floodFill.AddTile(floodTile.x, floodTile.y - 1, i + 1, floodTile.id);

                    lastTile = floodTile;
                }
            }

            while (lastTile.i > 1)
            {
                foreach (FloodTile floodTile in floodFill.GetFloodAt(lastTile.i - 1))
                {
                    if (floodTile.id == lastTile.parentID)
                    {
                        lastTile = floodTile;
                        break;
                    }
                }
            }

            if (lastTile.x < x)
            {
                if (direction.CurrentDirection == Direction.Right)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Up)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Down)
                    RotateRight();
                return;
            }

            if (lastTile.x > x)
            {
                if (direction.CurrentDirection == Direction.Left)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Up)
                    RotateRight();

                if (direction.CurrentDirection == Direction.Down)
                    RotateLeft();
                return;
            }

            if (lastTile.y < y)
            {
                if (direction.CurrentDirection == Direction.Down)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Left)
                    RotateRight();

                if (direction.CurrentDirection == Direction.Right)
                    RotateLeft();
                return;
            }

            if (lastTile.y > y)
            {
                if (direction.CurrentDirection == Direction.Up)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Left)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Right)
                    RotateRight();
                return;
            }
        }

        void GetTileInfo()
        {
            RegisterWalls();
            GetTileType();
        }

        void ManualMove()
        {
            ConsoleKey consoleKey = Console.ReadKey().Key;

            switch (consoleKey)
            {
                case ConsoleKey.F1:
                    IsAutonomous = (!IsAutonomous);
                    break;
                case ConsoleKey.LeftArrow:
                    RotateLeft();
                    break;
                case ConsoleKey.RightArrow:
                    RotateRight();
                    break;
                case ConsoleKey.UpArrow:
                    MoveTile();
                    break;
            }
        }

        void RotateNextTile()
        {
            NextVoidTiles nextVoidWalls = new NextVoidTiles((map[x - 1, y].rightWall == Wall.Yes), (map[x, y].rightWall == Wall.Yes), (map[x, y - 1].bottomWall == Wall.Yes), (map[x, y].bottomWall == Wall.Yes));
            NextVoidTiles nextVoidTiles = new NextVoidTiles((map[x - 1, y].tileType == TileType.Void), (map[x + 1, y].tileType == TileType.Void), (map[x, y - 1].tileType == TileType.Void), (map[x, y + 1].tileType == TileType.Void));

            if (!nextVoidWalls.Down && nextVoidTiles.Down)
            {
                if (direction.CurrentDirection == Direction.Up)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Left)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Right)
                    RotateRight();

                return;
            }

            if (!nextVoidWalls.Right && nextVoidTiles.Right)
            {
                if (direction.CurrentDirection == Direction.Left)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Down)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Up)
                    RotateRight();

                return;
            }

            if (!nextVoidWalls.Left && nextVoidTiles.Left)
            {
                if (direction.CurrentDirection == Direction.Right)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Up)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Down)
                    RotateRight();

                return;
            }

            if (!nextVoidWalls.Up && nextVoidTiles.Up)
            {
                if (direction.CurrentDirection == Direction.Down)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Right)
                    RotateLeft();

                if (direction.CurrentDirection == Direction.Left)
                    RotateRight();

                return;
            }
        }

        bool NearVoidTile()
        {
            NextVoidTiles nextVoidWalls = new NextVoidTiles((map[x - 1, y].rightWall == Wall.Yes), (map[x, y].rightWall == Wall.Yes), (map[x, y - 1].bottomWall == Wall.Yes), (map[x, y].bottomWall == Wall.Yes));
            NextVoidTiles nextVoidTiles = new NextVoidTiles((map[x - 1, y].tileType == TileType.Void), (map[x + 1, y].tileType == TileType.Void), (map[x, y - 1].tileType == TileType.Void), (map[x, y + 1].tileType == TileType.Void));

            int Paths = 0;

            if (!nextVoidWalls.Left && nextVoidTiles.Left)
                Paths++;

            if (!nextVoidWalls.Right && nextVoidTiles.Right)
                Paths++;

            if (!nextVoidWalls.Up && nextVoidTiles.Up)
                Paths++;

            if (!nextVoidWalls.Down && nextVoidTiles.Down)
                Paths++;

            return (Paths > 0) ? true : false;
        }

        void RotateToUnknownWall()
        {
            switch (direction.CurrentDirection)
            {
                case Direction.Up:
                    if (map[x, y].rightWall == Wall.Unknown)
                        RotateRight();
                    else
                        RotateLeft();
                    break;
                case Direction.Left:
                    if (map[x, y - 1].bottomWall == Wall.Unknown)
                        RotateRight();
                    else
                        RotateLeft();
                    break;
                case Direction.Right:
                    if (map[x, y].bottomWall == Wall.Unknown)
                        RotateRight();
                    else
                        RotateLeft();
                    break;
                default:
                    if (map[x - 1, y].rightWall == Wall.Unknown)
                        RotateRight();
                    else
                        RotateLeft();
                    break;
            }
        }

        #region Tile Information

        void GetTileType()
        {
            map[x, y].tileType = Arena.GetFloor();
        }

        void RegisterWalls()
        {
            switch (direction.CurrentDirection)
            {
                case Direction.Up:
                    RegisterWallFromTop();
                    break;
                case Direction.Left:
                    RegisterWallFromLeft();
                    break;
                case Direction.Right:
                    RegisterWallFromRight();
                    break;
                default:
                    RegisterWallFromBottom();
                    break;
            }
        }

        bool KnowsAllTiles()
        {
            return (map[x - 1, y].rightWall != Wall.Unknown && map[x, y - 1].bottomWall != Wall.Unknown && map[x, y].rightWall != Wall.Unknown && map[x, y].bottomWall != Wall.Unknown);
        }

        void RegisterWallFromTop()
        {
            Sonar sonar = Arena.GetTiles();

            map[x - 1, y].rightWall = (sonar.Left) ? Wall.Yes : Wall.No;
            map[x, y - 1].bottomWall = (sonar.Front) ? Wall.Yes : Wall.No;
            map[x, y].rightWall = (sonar.Right) ? Wall.Yes : Wall.No;
        }

        void RegisterWallFromLeft()
        {
            Sonar sonar = Arena.GetTiles();

            map[x - 1, y].rightWall = (sonar.Front) ? Wall.Yes : Wall.No;
            map[x, y - 1].bottomWall = (sonar.Right) ? Wall.Yes : Wall.No;
            map[x, y].bottomWall = (sonar.Left) ? Wall.Yes : Wall.No;
        }

        void RegisterWallFromBottom()
        {
            Sonar sonar = Arena.GetTiles();

            map[x - 1, y].rightWall = (sonar.Right) ? Wall.Yes : Wall.No;
            map[x, y].rightWall = (sonar.Left) ? Wall.Yes : Wall.No;
            map[x, y].bottomWall = (sonar.Front) ? Wall.Yes : Wall.No;
        }

        void RegisterWallFromRight()
        {
            Sonar sonar = Arena.GetTiles();

            map[x, y - 1].bottomWall = (sonar.Left) ? Wall.Yes : Wall.No;
            map[x, y].rightWall = (sonar.Front) ? Wall.Yes : Wall.No;
            map[x, y].bottomWall = (sonar.Right) ? Wall.Yes : Wall.No;
        }

        #endregion

        #region Movement

        void MoveTile()
        {
            if (!Arena.GetTiles().Front)
            {
                MoveForward();
                Arena.MoveTile();
            }
                
        }

        void MoveForward()
        {
            switch (direction.CurrentDirection)
            {
                case Direction.Up:
                    MoveUp();
                    break;
                case Direction.Right:
                    MoveRight();
                    break;
                case Direction.Down:
                    MoveDown();
                    break;
                default:
                    MoveLeft();
                    break;
            }
        }

        void RotateLeft()
        {
            Arena.RotateLeft();
            direction.RotateLeft();
        }

        void RotateRight()
        {
            Arena.RotateRight();
            direction.RotateRight();
        }

        void MoveUp()
        {
            y--;
        }

        void MoveRight()
        {
            x++;
        }

        void MoveDown()
        {
            y++;
        }

        void MoveLeft()
        {
            x--;
        }

        #endregion
    }
}
