using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chess
{
    class Rook : Piece
    {
        public bool firstMove { get; private set; }
        public Rook(Tile tile, PieceColour colour) : base(tile) 
        {
            this.pieceType = PieceType.rook;
            this.colour = colour;
            this.firstMove = true;
        }

        public override void DisplayValidMove(Tile[,] board)
        {   
            validNextMove = PreventKingInDanger(board);
            if (validNextMove != null)
            {
                foreach (Tile tile in validNextMove) // check is king in danger first before normal movement
                {
                    tile.picture.BackColor = Color.LightBlue;
                }
            }
        }

        public override void CheckKing(Tile[,] board)
        {
            firstMove = false;
        }

        private protected override List<Tile> FindValidNextMove(Tile[,] board)
        {
            // recursive call, this -> king beging check -> opponent all valid move -> this
            validNextMove.Clear();
            checkPath.Clear();
            bool foundCheckPath = false;

            for (int x = this.tile.x; x < 7; x++) // check right horizontal
            {
                Tile tile = board[x + 1, this.tile.y];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else
                {
                    if (this.colour != tile.piece.colour) // is opponent pieces
                    {
                        validNextMove.Add(tile);
                        if (tile.piece.pieceType == PieceType.king)
                        {
                            checkPath.Add(tile);
                            foundCheckPath = true;
                        }
                        else if (!foundCheckPath) checkPath.Clear();
                    }
                    break;
                }
            }
            if (!foundCheckPath) checkPath.Clear();
            for (int x = this.tile.x; x >= 1; x--) // check left horizontal
            {
                Tile tile = board[x - 1, this.tile.y];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else
                {
                    if (this.colour != tile.piece.colour) // is opponent pieces
                    {
                        validNextMove.Add(tile);
                        if (tile.piece.pieceType == PieceType.king)
                        {
                            checkPath.Add(tile);
                            foundCheckPath = true;
                        }
                        else if (!foundCheckPath) checkPath.Clear();
                    }
                    break;
                }
            }
            if (!foundCheckPath) checkPath.Clear();
            for (int y = this.tile.y; y < 7; y++) // check down vertical
            {
                Tile tile = board[this.tile.x, y + 1];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else
                {
                    if (this.colour != tile.piece.colour) // is opponent pieces
                    {
                        validNextMove.Add(tile);
                        if (tile.piece.pieceType == PieceType.king)
                        {
                            checkPath.Add(tile);
                            foundCheckPath = true;
                        }
                        else if (!foundCheckPath) checkPath.Clear();
                    }
                    break;
                }
            }
            if (!foundCheckPath) checkPath.Clear();
            for (int y = this.tile.y; y >= 1; y--) // check up vertical
            {
                Tile tile = board[this.tile.x, y - 1];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else
                {
                    if (this.colour != tile.piece.colour) // is opponent pieces
                    {
                        validNextMove.Add(tile);
                        if (tile.piece.pieceType == PieceType.king)
                        {
                            checkPath.Add(tile);
                            foundCheckPath = true;
                        }
                        else if (!foundCheckPath) checkPath.Clear();
                    }
                    break;
                }
            }

            if (checkPath.Count > 0) checkPath.Add(board[this.tile.x, this.tile.y]);
            return validNextMove;
        }
    }
}
