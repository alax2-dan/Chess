using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chess
{
    class Knight : Piece
    {
        public Knight(Tile tile, PieceColour colour) : base(tile)
        {
            this.pieceType = PieceType.knight;
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
            int x = this.tile.x;
            int y = this.tile.y;
            validNextMove.Clear();
            if (IsValidPoistion(x - 2, y - 1)) // left up
            {
                Tile tile = board[x - 2, y - 1];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }
            if (IsValidPoistion(x - 2, y + 1)) // left down
            {
                Tile tile = board[x - 2, y + 1];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }
            if (IsValidPoistion(x + 2, y - 1)) // right up 
            {
                Tile tile = board[x + 2, y - 1];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }
            if (IsValidPoistion(x + 2, y + 1)) // right down
            {
                Tile tile = board[x + 2, y + 1];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }
            if (IsValidPoistion(x - 1, y - 2)) // up left
            {
                Tile tile = board[x - 1, y - 2];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }
            if (IsValidPoistion(x + 1, y - 2)) // up right
            {
                Tile tile = board[x + 1, y - 2];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }
            if (IsValidPoistion(x - 1, y + 2)) // down left
            {
                Tile tile = board[x - 1, y + 2];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }
            if (IsValidPoistion(x + 1, y + 2)) // down right
            {
                Tile tile = board[x + 1, y + 2];
                if (tile.piece == null || this.colour != tile.piece.colour)
                {
                    validNextMove.Add(tile);
                }
                Piece piece = tile.piece;
                if (piece != null && this.colour != piece.colour && piece.pieceType == PieceType.king) checkPath.Add(tile);
            }

            if (checkPath.Count > 0) checkPath.Add(board[this.tile.x, this.tile.y]);
            return validNextMove;
        }
    }
}
