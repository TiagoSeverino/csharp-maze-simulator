using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_Maze
{
    class FloodFill
    {
        public FloodTile[,] tile;
        int LastTileNumber = 0;
        public int id = 0;

        public FloodFill(int Width, int Height)
        {
            tile = new FloodTile[Width, Height];
        }

        public void AddTile(int x, int y, int i, int ParentID)
        {
            if (tile[x, y] == null)
            {
                if (i > LastTileNumber)
                    LastTileNumber = i;
                id++;
                tile[x, y] = new FloodTile(x, y, i, ParentID, id);
            }
        }

        public List<FloodTile> GetFloodAt(int i)
        {
            List<FloodTile> floodTiles = new List<FloodTile>();

            foreach(FloodTile floodTile in tile)
                if(floodTile != null)
                    if (floodTile.i == i)
                        floodTiles.Add(floodTile);

            return floodTiles;
        }

        public int GetLastTileNumber()
        {
            return LastTileNumber;
        }
    }

    class FloodTile
    {
        public int x, y, i, parentID, id;

        public FloodTile(int X, int Y, int I, int ParentID, int ID)
        {
            x = X;
            y = Y;
            i = I;
            parentID = ParentID;
            id = ID;
        }
    }
}
