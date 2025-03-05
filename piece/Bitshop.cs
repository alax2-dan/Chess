using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chess
{
    class Bitshop : Piece
    {
        public Bitshop(Tile tile, PieceColour colour) : base(tile)
        {
            this.pieceType = PieceType.bitshop;
            this.colour = colour;
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
            return;
        }

        private protected override List<Tile> FindValidNextMove(Tile[,] board)
        {
            validNextMove.Clear();
            checkPath.Clear();
            bool foundCheckPath = false;

            for (int x = this.tile.x, y = this.tile.y; x <= 7 && y >= 0; x++, y--) // right up
            {
                if (!IsValidPoistion(x + 1, y - 1)) break;
                Tile tile = board[x + 1, y - 1];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else if (this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                    if (tile.piece.pieceType == PieceType.king)
                    {
                        checkPath.Add(tile);
                        foundCheckPath = true;
                    }
                    else if (!foundCheckPath) checkPath.Clear();
                    break;
                }
                else break;
            }
            if (!foundCheckPath) checkPath.Clear();
            for (int x = this.tile.x, y = this.tile.y; x >= 0 && y >= 0; x--, y--) // left up
            {
                if (!IsValidPoistion(x - 1, y - 1)) break;
                Tile tile = board[x - 1, y - 1];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else if (this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                    if (tile.piece.pieceType == PieceType.king)
                    {
                        checkPath.Add(tile);
                        foundCheckPath = true;
                    }
                    else if (!foundCheckPath) checkPath.Clear();
                    break;
                }
                else break;
            }
            if (!foundCheckPath) checkPath.Clear();
            for (int x = this.tile.x, y = this.tile.y; x >= 0 && y <= 7; x--, y++) // left down
            {
                if (!IsValidPoistion(x - 1, y + 1)) break;
                Tile tile = board[x - 1, y + 1];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else if (this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                    if (tile.piece.pieceType == PieceType.king)
                    {
                        checkPath.Add(tile);
                        foundCheckPath = true;
                    }
                    else if (!foundCheckPath) checkPath.Clear();
                    break;
                }
                else break;
            }
            if (!foundCheckPath) checkPath.Clear();
            for (int x = this.tile.x, y = this.tile.y; x <= 7 && y <= 7; x++, y++) // right down
            {
                if (!IsValidPoistion(x + 1, y + 1)) break;
                Tile tile = board[x + 1, y + 1];
                if (tile.piece == null)
                {
                    validNextMove.Add(tile);
                    if (!foundCheckPath) checkPath.Add(tile);
                }
                else if (this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                    if (tile.piece.pieceType == PieceType.king)
                    {
                        checkPath.Add(tile);
                        foundCheckPath = true;
                    }
                    else if (!foundCheckPath) checkPath.Clear();
                    break;
                }
                else break;
            }

            if (checkPath.Count > 0) checkPath.Add(board[this.tile.x, this.tile.y]);
            return validNextMove;
        }
    }
}
