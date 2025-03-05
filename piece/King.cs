using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chess
{
    class King : Piece
    {
        public bool firstMove { get; private set; }
        Tile source;
        Tile target;
        public King(Tile tile, PieceColour colour) : base(tile)
        {
            this.pieceType = PieceType.king;
            this.colour = colour;
            this.firstMove = true;
        }


        public override void DisplayValidMove(Tile[,] board)
        {
            validNextMove = PreventKingInDanger(board);
            foreach (Tile validNextMove in validNextMove)
            {
                validNextMove.picture.BackColor = Color.LightBlue;
            }

            if (Piece.AllowCastling(board, this, true)) // allow castle to right
            {
                if (this.colour == PieceColour.white)
                {
                    validNextMove.Add(board[6, 7]);
                    board[6, 7].picture.BackColor = Color.Purple;
                }
                else
                {
                    validNextMove.Add(board[6, 0]);
                    board[6, 0].picture.BackColor = Color.Purple;
                }
            }
            if (Piece.AllowCastling(board, this, false)) // allow castle to left
            {
                if (this.colour == PieceColour.white)
                {
                    validNextMove.Add(board[2, 7]);
                    board[2, 7].picture.BackColor = Color.Purple;
                }
                else
                {
                    validNextMove.Add(board[2, 0]);
                    board[2, 0].picture.BackColor = Color.Purple;
                }
            }
        }

        public override void Move(Tile source, Tile target, Tile[,] board)
        {
            // move the image
            target.picture.Image = source.picture.Image;
            source.picture.Image = null;

            source.piece.tile = target;
            target.piece = source.piece;

            PieceType sourceType = source.piece.pieceType;
            source.piece = null;
            target.piece.pieceType = sourceType;

            this.source = source;
            this.target = target;
            target.piece.CheckKing(board); // check is the piece checking the opponent's king
        }

        private void HandleCastling(Tile source, Tile clickedPicture, Tile[,] board)
        {
            Tile rook = board[0, 0]; // coorespond rook for castling 
            Tile destination = board[0, 0]; // where the rook locate after castling
            if (clickedPicture.x == 6 && clickedPicture.y == 7 && clickedPicture.piece.colour == PieceColour.white) // if castle to right and is white
            {
                rook = board[7, 7];
                destination = board[5, 7];
            }
            else if (clickedPicture.x == 6 && clickedPicture.y == 0 && clickedPicture.piece.colour == PieceColour.black) // to right and is black
            {
                rook = board[7, 0];
                destination = board[5, 0];
            }
            else if (clickedPicture.x == 2 && clickedPicture.y == 7 && clickedPicture.piece.colour == PieceColour.white) // if castle to left and is white
            {
                rook = board[0, 7];
                destination = board[3, 7];
            }
            else // to left and is black
            {
                rook = board[0, 0];
                destination = board[3, 0];
            }
            // move rook image
            destination.picture.Image = rook.picture.Image;
            rook.picture.Image = null;

            rook.piece.tile = destination;
            destination.piece = rook.piece;
            destination.piece.pieceType = PieceType.rook;
            rook.piece = null;

            ClearBoard(board);
            // see is opponent king being check after castling
            PieceColour oppoentKingColour = (target.piece.colour == PieceColour.white) ? PieceColour.black : PieceColour.white;
            if (Piece.IsKingBeingChecked(board, oppoentKingColour).Count > 0)
            {
                Piece.GetKingPoistion(board, oppoentKingColour).picture.BackColor = Color.Red;
            }
        }

        private protected override List<Tile> FindValidNextMove(Tile[,] board)
        {
            validNextMove.Clear();
            for (int x = this.tile.x - 1; x <= this.tile.x + 1; x++)
            {
                for (int y = this.tile.y - 1; y <= this.tile.y + 1; y++)
                {
                    if (IsValidPoistion(x, y))
                    {
                        if (board[x, y].piece == null)
                        {
                            validNextMove.Add(board[x, y]);
                        }
                        else if (this.colour != board[x, y].piece.colour)
                        {
                            validNextMove.Add(board[x, y]);
                            break;
                        }
                    }
                }
            }
            return validNextMove;
        }

        private protected override List<Tile> PreventKingInDanger(Tile[,] board)
        {
            List<Tile> normalMove = FindValidNextMove(board);
            List<Tile> safetyMove = new List<Tile>();
            for (int x = this.tile.x - 1; x <= this.tile.x + 1; x++)
            {
                for (int y = this.tile.y - 1; y <= this.tile.y + 1; y++)
                {
                    if (IsValidPoistion(x, y))
                    {
                        if (board[x, y].piece == null)
                        {
                            Piece current = board[this.tile.x, this.tile.y].piece;
                            this.tile.piece = null;
                            board[x, y].piece = current;
                            if (IsKingBeingChecked(board, this.colour).Count > 0) safetyMove.Add(board[x, y]);
                            board[x, y].piece = null;
                            board[this.tile.x, this.tile.y].piece = current;
                        }
                        else if (board[x, y].piece?.colour != this.colour)
                        {
                            Piece current = board[this.tile.x, this.tile.y].piece;
                            Piece piece = board[x, y].piece;
                            this.tile.piece = null;
                            board[x, y].piece = current;
                            if (IsKingBeingChecked(board, this.colour).Count > 0) safetyMove.Add(board[x, y]);
                            board[x, y].piece = piece;
                            board[this.tile.x, this.tile.y].piece = current;
                        }
                    }
                }
            }
            return validNextMove.Where(x => !safetyMove.Contains(x)).ToList();
        }

        public override void CheckKing(Tile[,] board)
        {
            firstMove = false;
            if (target.picture.BackColor == Color.Purple) HandleCastling(source, target, board);
            else ClearBoard(board);
        }
    }
}
