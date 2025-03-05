using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chess
{
    class Queen : Piece
    {
        public Queen(Tile tile, PieceColour colour) : base(tile)
        {
            this.pieceType = PieceType.queen;
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

            if (!foundCheckPath) checkPath.Clear();
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
