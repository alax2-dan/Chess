using Chess.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
    public partial class gameForm : Form
    {
        static Tile[,] board = new Tile[8, 8];
        Tile selectedPieceTile; // store which piece the player selected
        bool whiteTurn = true;
        Tile clickedTile = null;
        public gameForm()
        {
            InitializeComponent();
            drawBoard();
            setUpPiece();
            Pawn.SetupPawnPromotionPicture();
        }

        private void drawBoard() // draw the chess board
        {
            int tileWidth = this.Width / 8; // 94
            int tileHeight = this.Height / 8; // 81

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Size = new Size(tileWidth, tileHeight);
                    pictureBox.Location = new Point(x * tileWidth, y * tileHeight);
                    pictureBox.MouseClick += Pieces_Click;
                    if ((x + y) % 2 == 0)
                    {
                        pictureBox.BackColor = Color.White;
                    }
                    else
                    {
                        pictureBox.BackColor = Color.Black;
                    }
                    this.Controls.Add(pictureBox);
                    board[x, y] = new Tile(pictureBox, x, y);
                }
            }
        }

        private void Pieces_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || Pawn.IsPromoting()) return;
            PictureBox clickedPicture = sender as PictureBox;
            foreach (Tile tile in board)
            {
                if (tile.picture == clickedPicture)
                {
                    clickedTile = tile;
                    Pawn.UpdateTarget(clickedTile);
                    break;
                }
            }
            if (whiteTurn && (clickedTile.piece != null) && clickedTile.piece.colour == PieceColour.black
                && clickedTile.picture.BackColor != Color.LightBlue) return; 
            if (clickedTile.piece != null && clickedTile.piece.colour == ColourTurn() && !Pawn.IsPromoting())
            {
                DisplayValidMove(clickedTile);
                selectedPieceTile = clickedTile;
            }
            else
            {
                if (clickedTile.picture.BackColor == Color.LightBlue || clickedTile.picture.BackColor == Color.Purple)
                {
                    selectedPieceTile.piece.Move(selectedPieceTile, clickedTile, board);
                    whiteTurn = whiteTurn ? false : true;
                }
                else
                {
                    ClearBoard();
                }
            }
        }

        private void DisplayValidMove(Tile tile)
        {
            ClearBoard();
            tile.piece.DisplayValidMove(board);
        }

        public static void ClearBoard()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x, y].picture.BackColor == Color.Red) continue;
                    if ((x + y) % 2 == 0)
                    {
                        board[x, y].picture.BackColor = Color.White;
                    }
                    else
                    {
                        board[x, y].picture.BackColor = Color.Black;
                    }
                }
            }
        }

        private void setUpPiece()
        {
            // set up pawn
            for (int x = 0; x < 8; x++)
            {
                board[x, 1].piece = new Pawn(board[x, 1], PieceColour.black, this, board);
                board[x, 6].piece = new Pawn(board[x, 6], PieceColour.white, this, board);

                board[x, 1].picture.Image = Resources.PawnB_small_;
                board[x, 6].picture.Image = Resources.PawnW_small_;
            }

            // set up rook
            for (int x = 0; x < 8; x += 7)
            {
                board[x, 0].piece = new Rook(board[x, 0], PieceColour.black);
                board[x, 7].piece = new Rook(board[x, 7], PieceColour.white);

                board[x, 0].picture.Image = Resources.RookB_small_;
                board[x, 7].picture.Image = Resources.RookW_small_;
            }

            // set up knight
            for (int x = 1; x < 8; x += 5)
            {
                board[x, 0].piece = new Knight(board[x, 0], PieceColour.black);
                board[x, 7].piece = new Knight(board[x, 7], PieceColour.white);

                board[x, 0].picture.Image = Resources.KnightB_small_;
                board[x, 7].picture.Image = Resources.KnightW_small_;
            }

            // set up bitshop
            for (int x = 2; x < 8; x += 3)
            {
                board[x, 0].piece = new Bitshop(board[x, 0], PieceColour.black);
                board[x, 7].piece = new Bitshop(board[x, 7], PieceColour.white);

                board[x, 0].picture.Image = Resources.BishopB_small_;
                board[x, 7].picture.Image = Resources.BishopW_small_;
            }

            // set up queen
            board[3, 0].piece = new Queen(board[3, 0], PieceColour.black);
            board[3, 7].piece = new Queen(board[3, 7], PieceColour.white);

            board[3, 0].picture.Image = Resources.QueenB_small_;
            board[3, 7].picture.Image = Resources.QueenW_small_;

            // set up king
            board[4, 0].piece = new King(board[4, 0], PieceColour.black);
            board[4, 7].piece = new King(board[4, 7], PieceColour.white);

            board[4, 0].picture.Image = Resources.KingB_small_;
            board[4, 7].picture.Image = Resources.KingW_small_;
        }

        private PieceColour ColourTurn() // return which player turn now
        {
            return whiteTurn ? PieceColour.white : PieceColour.black;
        }
    }
}
