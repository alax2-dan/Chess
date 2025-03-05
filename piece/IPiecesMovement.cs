using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    interface IPiecesMovement
    {
        void DisplayValidMove(Tile[,] board);
        void CheckKing(Tile[,] board);
        void Move(Tile source, Tile target, Tile[,] board);
    }
}
