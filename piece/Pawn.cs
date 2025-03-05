using Chess.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chess
{
    class Pawn : Piece
    {
        public bool firstMove { get; private set; }
        static gameForm GameForm;
        static bool promoting = false;
        static PictureBox[] pawnPromotion = new PictureBox[4];
        static Tile[,] Board;
        static Tile Target; // clicked tile by the player 
        public Pawn(Tile tile, PieceColour colour, gameForm gameForm, Tile[,] board) : base(tile)
        {
            this.pieceType = PieceType.pawn;
            this.colour = colour;
            firstMove = true;
            GameForm = gameForm;
            Board = board;
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
            PawnPromotionCheck();
        }

        private void PawnPromotionCheck()
        {
            if (this.colour == PieceColour.white && this.tile.y == 0) // build the promote picture box array
            {
                for (int i = 0; i < pawnPromotion.Length; i++)
                {
                    PictureBox picture = pawnPromotion[i];
                    picture.Visible = true;
                    picture.MouseClick += PawnPromotionClick;
                    if (i == 0)
                    {
                        picture.Image = Resources.QueenW_small_;
                        picture.Tag = "queen";
                    }
                    else if (i == 1)
                    {
                        picture.Image = Resources.RookW_small_;
                        picture.Tag = "rook";
                    }
                    else if (i == 2)
                    {
                        picture.Image = Resources.BishopW_small_;
                        picture.Tag = "bitshop";
                    }
                    else
                    {
                        picture.Image = Resources.KnightW_small_;
                        picture.Tag = "knight";
                    }
                }
                promoting = true;
            }
            if (this.colour == PieceColour.black && this.tile.y == 7)
            {
                for (int i = 0; i < pawnPromotion.Length; i++)
                {
                    PictureBox picture = pawnPromotion[i];
                    picture.Visible = true;
                    picture.MouseClick += PawnPromotionClick;
                    if (i == 0)
                    {
                        picture.Image = Resources.QueenB_small_;
                        picture.Tag = "queen";
                    }
                    else if (i == 1)
                    {
                        picture.Image = Resources.RookB_small_;
                        picture.Tag = "rook";
                    }
                    else if (i == 2)
                    {
                        picture.Image = Resources.BishopB_small_;
                        picture.Tag = "bitshop";
                    }
                    else
                    {
                        picture.Image = Resources.KnightB_small_;
                        picture.Tag = "knight";
                    }
                }
                promoting = true;
            }
        }

        private void PawnPromotionClick(object sender, MouseEventArgs e)
        {
            PictureBox clickedPicture = sender as PictureBox;
            Target.picture.Image = clickedPicture.Image; // change pawn image to promotion image
            Target.piece.tile = Target;

            PieceColour pieceColour = Target.y == 0 ? PieceColour.white : PieceColour.black;

            string pieceType = clickedPicture.Tag.ToString();
            switch (pieceType)
            {
                case "queen":
                    Target.piece = new Queen(Target, pieceColour);
                    break;
                case "rook":
                    Target.piece = new Rook(Target, pieceColour);
                    break;
                case "bitshop":
                    Target.piece = new Bitshop(Target, pieceColour);
                    break;
                case "knight":
                    Target.piece = new Knight(Target, pieceColour);
                    break;
            }
            foreach (var picture in pawnPromotion)
            {
                picture.Visible = false;
                picture.MouseClick -= PawnPromotionClick;
            }
            Target.piece.CheckKing(Board);
            promoting = false;
        }

        public static void handleEnPassantmove(Tile[,] board, Piece clickedPiece)
        {
            if (clickedPiece.pieceType != PieceType.pawn) return;

            if (clickedPiece.colour == PieceColour.white)
            {
                Tile opponentPawnTile = board[clickedPiece.tile.x, clickedPiece.tile.y + 1];
                if (opponentPawnTile.piece?.colour == PieceColour.black && opponentPawnTile.piece?.pieceType == PieceType.pawn)
                {
                    opponentPawnTile.picture.Image = null;
                    opponentPawnTile.piece = null;
                }
            }
            else
            {
                Tile opponentPawnTile = board[clickedPiece.tile.x, clickedPiece.tile.y - 1];
                if (opponentPawnTile.piece?.colour == PieceColour.white && opponentPawnTile.piece?.pieceType == PieceType.pawn)
                {
                    opponentPawnTile.picture.Image = null;
                    opponentPawnTile.piece = null;
                }
            }
        }

        public static void SetupPawnPromotionPicture()
        {
            PictureBox tile = Board[7, 3].picture;
            int x = tile.Left + tile.Width + 10;
            int y = tile.Top;
            for (int i = 1; i < pawnPromotion.Length + 1; i++)
            {
                PictureBox picture = new PictureBox();
                picture.Visible = false;
                picture.Left = x + 81 * i;
                picture.Top = y;
                picture.Height = 81;
                pawnPromotion[i - 1] = picture;
                GameForm.Controls.Add(pawnPromotion[i - 1]);
            }
        }

        private protected override List<Tile> FindValidNextMove(Tile[,] board)
        {
            int x = this.tile.x;
            int y = this.tile.y;
            validNextMove.Clear();
            if (this.colour == PieceColour.white)
            {
                // pawn eat pieces
                if (IsValidPoistion(x - 1, y - 1)) // eat left
                {
                    Piece piece = board[x - 1, y - 1].piece;
                    if (piece == null) // en passant check
                    {
                        Piece opponentpawn = board[this.tile.x - 1, this.tile.y].piece;
                        if (opponentpawn?.pieceType == PieceType.pawn && this.colour != opponentpawn?.colour)
                        {
                            validNextMove.Add(board[x - 1, y - 1]);
                            checkPath.Add(board[x - 1, y - 1]);
                        }
                    }
                    else if (this.colour != piece.colour)
                    {
                        validNextMove.Add(board[x - 1, y - 1]);
                        checkPath.Add(board[x - 1, y - 1]);
                    }
                }
                if (IsValidPoistion(x + 1, y - 1)) // eat right
                {
                    Piece piece = board[x + 1, y - 1].piece;
                    if (piece == null) // en passant check
                    {
                        Piece opponentpawn = board[this.tile.x + 1, this.tile.y].piece;
                        if (opponentpawn?.pieceType == PieceType.pawn && this.colour != opponentpawn?.colour)
                        {
                            validNextMove.Add(board[x + 1, y - 1]);
                            checkPath.Add(board[x + 1, y - 1]);
                        }
                    }
                    else if (piece != null && this.colour != piece.colour)
                    {
                        validNextMove.Add(board[x + 1, y - 1]);
                        checkPath.Add(board[x + 1, y - 1]);
                    }
                }

                // pawn move
                if (firstMove)
                {
                    if (IsValidPoistion(x, y - 1) && board[x, y - 1].piece == null)
                    {
                        validNextMove.Add(board[x, y - 1]);
                        if (board[x, y - 2].piece == null) validNextMove.Add(board[x, y - 2]);
                    }
                }
                else
                {
                    if (IsValidPoistion(x, y - 1) && board[x, y - 1].piece == null) validNextMove.Add(board[x, y - 1]);
                }
            }
            else
            {
                // pawn eat pieces
                if (IsValidPoistion(x - 1, y + 1)) // eat left
                {
                    Piece piece = board[x - 1, y + 1].piece;
                    if (piece == null) // en passant check
                    {
                        Piece opponentpawn = board[this.tile.x - 1, this.tile.y].piece;
                        if (opponentpawn?.pieceType == PieceType.pawn && this.colour != opponentpawn?.colour)
                        {
                            validNextMove.Add(board[x - 1, y + 1]);
                            checkPath.Add(board[x - 1, y + 1]);
                        }
                    }
                    else if (piece != null && this.colour != piece.colour)
                    {
                        validNextMove.Add(board[x - 1, y + 1]);
                        checkPath.Add(board[x - 1, y + 1]);
                    }
                }
                if (IsValidPoistion(x + 1, y + 1)) // eat right
                {
                    Piece piece = board[x + 1, y + 1].piece;
                    if (piece == null) // en passant check
                    {
                        Piece opponentpawn = board[this.tile.x + 1, this.tile.y].piece;
                        if (opponentpawn?.pieceType == PieceType.pawn && this.colour != opponentpawn?.colour)
                        {
                            validNextMove.Add(board[x + 1, y + 1]);
                            checkPath.Add(board[x + 1, y + 1]);
                        }
                    }
                    else if (piece != null && this.colour != piece.colour)
                    {
                        validNextMove.Add(board[x + 1, y + 1]);
                        checkPath.Add(board[x + 1, y + 1]);
                    }
                }

                // pawn move
                if (firstMove)
                {
                    if (IsValidPoistion(x, y + 1) && board[x, y + 1].piece == null)
                    {
                        validNextMove.Add(board[x, y + 1]);
                        if (board[x, y + 2].piece == null) validNextMove.Add(board[x, y + 2]);
                    }
                }
                else
                {
                    if (IsValidPoistion(x, y + 1) && board[x, y + 1].piece == null) validNextMove.Add(board[x, y + 1]);
                }
            }

            if (checkPath.Count > 0) checkPath.Add(board[this.tile.x, this.tile.y]);
            return validNextMove;
        }

        public static void UpdateTarget(Tile target)
        {
            Target = target;
        }

        public static bool IsPromoting()
        {
            return promoting;
        }
    }
}
