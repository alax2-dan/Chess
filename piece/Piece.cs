using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Chess
{
    abstract class Piece : IPiecesMovement
    {
        public PieceType pieceType { get; set; }
        public PieceColour colour { get; set; }
        public Tile tile { get; set; }
        public List<Tile> validNextMove { get; private protected set; }
        public List<Tile> checkPath { get; private set; } // the tile path that checks the king, used to prevent piece move cause king in danger, it is 
                                                          // added with the same logic as valid next move

        private static HashSet<List<Tile>> opponentAllValidMove = new HashSet<List<Tile>>();

        public Piece(Tile tile)
        {
            this.tile = tile;
            this.validNextMove = new List<Tile>();
            this.checkPath = new List<Tile>();
        }

        public virtual void Move(Tile source, Tile target, Tile[,] board)
        {
            // move the image
            target.picture.Image = source.picture.Image;
            source.picture.Image = null;

            source.piece.tile = target;
            target.piece = source.piece;
            PieceType sourceType = source.piece.pieceType;
            source.piece = null;
            target.piece.pieceType = sourceType;

            Pawn.handleEnPassantmove(board, target.piece);

            ClearBoard(board);

            PieceColour oppoentKingColour = (target.piece.colour == PieceColour.white) ? PieceColour.black : PieceColour.white;
            GetOpponentAllValidMove(board, target.piece.colour); // fillin opponent check path, and so when your turn check path is not empty (if is checking)

            if (Piece.IsKingBeingChecked(board, oppoentKingColour).Count > 0)
            {
                Piece.GetKingPoistion(board, oppoentKingColour).picture.BackColor = Color.Red; // opponent king's tile turn to red, indicate checking 
                if (opponentAllValidMove.All(x => x.Count == 0))
                {
                    MessageBox.Show($"{target.piece.colour} won!");
                }
            }
            else
            {
                if (opponentAllValidMove.All(x => x.Count == 0))
                {
                    MessageBox.Show($"It is stalemate!");
                }
            }

            target.piece.CheckKing(board); // check is the piece checking the opponent's king
        }

        private protected static void PreventKingMoveToDangerPoistion(Tile[,] board, Tile kingTile, PieceColour pieceColour) // used in king class
        {
            // handle not allowing piece move if checked after move
            Piece current = board[kingTile.x, kingTile.y].piece;
            kingTile.piece = null; // make the piece "disappear" on the board

            GetOpponentAllValidMove(board, pieceColour);
            board[kingTile.x, kingTile.y].piece = current; // make it "appear" back
            List<Tile> kingValidMove = kingTile.piece.FindValidNextMove(board); // find king valid move (without consider safety)
            kingValidMove.RemoveAll(tile => opponentAllValidMove.Any(tileList => tileList.Contains(tile)));
        }

        private protected static List<Tile> IsKingBeingChecked(Tile[,] board, PieceColour kingColour) // return the tile is ckecking the king
        {
            Tile kingTile = null;
            foreach (Tile tile in board)
            {
                if (tile.piece?.colour == kingColour && tile.piece.pieceType == PieceType.king)
                {
                    kingTile = tile;
                    break;
                }
            }
            return ExploreKingDiagonal(board, kingTile)
                   .Concat(ExploreKingHorVertical(board, kingTile))
                   .Concat(ExploreKingKnight(board, kingTile)).ToList();
        }

        private protected static bool AllowCastling(Tile[,] board, King king, bool castlingToRight)
        {
            if (!king.firstMove) return false;

            if (Piece.IsKingBeingChecked(board, king.colour).Count > 0) return false;

            GetOpponentAllValidMove(board, king.colour);
            Tile tile1 = board[0, 0];
            Tile tile2 = board[0, 0];
            Tile tile3 = board[0, 0];
            Tile rookTile = board[0, 0];
            if (king.colour == PieceColour.white && castlingToRight)
            {
                tile1 = board[5, 7];
                tile2 = board[6, 7];
                rookTile = board[7, 7];
            }
            else if (king.colour == PieceColour.black && castlingToRight)
            {
                tile1 = board[5, 0];
                tile2 = board[6, 0];
                rookTile = board[7, 0];
            }
            else if (king.colour == PieceColour.white && !castlingToRight)
            {
                tile1 = board[2, 7];
                tile2 = board[3, 7];
                tile3 = board[1, 7];
                rookTile = board[0, 7];
            }
            else
            {
                tile1 = board[2, 0];
                tile2 = board[3, 0];
                tile3 = board[1, 0];
                rookTile = board[0, 0];
            }
            if (castlingToRight && (tile1.piece != null || tile2.piece != null || rookTile.piece == null)) return false;
            if (!castlingToRight && (tile1.piece != null || tile2.piece != null || tile3.piece != null || rookTile.piece == null)) return false;
            if (rookTile.piece.pieceType == PieceType.rook)
            {
                Rook rook = rookTile.piece as Rook;
                if (!rook.firstMove) return false;
            }
            return !opponentAllValidMove.Any(tileList => tileList.Contains(tile1) || tileList.Contains(tile2));
        }

        private static void GetOpponentAllValidMove(Tile[,] board, PieceColour pieceColour)
        {
            opponentAllValidMove.Clear();
            foreach (Tile tile in board)
            {
                if (tile.piece != null && tile.piece.colour != pieceColour)
                {
                    opponentAllValidMove.Add(tile.piece.GetValidnextMove(board));
                }
            }
        }

        private protected virtual List<Tile> GetValidnextMove(Tile[,] board) // return all valid move of a piece 
        {
            return PreventKingInDanger(board);
        }

        private protected virtual List<Tile> PreventKingInDanger(Tile[,] board)
        {
            // handle not allowing piece move if checked after move
            Piece current = board[this.tile.x, this.tile.y].piece;
            this.tile.piece = null; // make the piece "disappear" on the board
            List<Tile> opponentTile = Piece.IsKingBeingChecked(board, this.colour); // stores the tile that is checking the king
            board[this.tile.x, this.tile.y].piece = current;
            if (opponentTile.Count > 1) // two pieces attacking the king at the same time
            {
                return null; // the piece can't move
            }
            else if (opponentTile.Count > 0) 
            {
                Tile checkTile = opponentTile[0]; // the tile that is attacking the king
                List<Tile> CheckPath = checkTile.piece.checkPath; // filled valid move list already in "is king being check"
                List<Tile> currentValidMove = FindValidNextMove(board);
                return CheckPath.Intersect(currentValidMove).ToList(); // new valid move after considering king safety
            }
            return FindValidNextMove(board);
        } // return all valid move of a piece 

        private protected void ClearBoard(Tile[,] board)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
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
        } // clear the board with red

        public virtual void DisplayValidMove(Tile[,] board)
        {
            return;
        }  // exe when player pressed on a piece

        private static List<Tile> ExploreKingDiagonal(Tile[,] board, Tile kingTile) // didn't consider pawn 
        {
            List<Tile> checkTile = new List<Tile>(4);
            for (int x = kingTile.x + 1, y = kingTile.y - 1; x <= 7 && y >= 0; x++, y--) // right up
            {
                Piece piece = board[x, y].piece;
                if (piece != null) 
                {
                    if(piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.pawn)
                    {
                        if (board[kingTile.x + 1, kingTile.y - 1].piece == piece)
                        {
                            checkTile.Add(board[kingTile.x + 1, kingTile.y - 1]);
                        }
                        else break;
                    }
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.bitshop)
                    {
                        board[x, y].piece.FindValidNextMove(board);
                        checkTile.Add(board[x, y]);
                        break;
                    }
                }
            }
            for (int x = kingTile.x - 1, y = kingTile.y - 1; x >= 0 && y >= 0; x--, y--) // left up
            {
                Piece piece = board[x, y].piece;
                if (piece != null)
                {
                    if (piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.pawn)
                    {
                        if (board[kingTile.x - 1, kingTile.y - 1].piece == piece)
                        {
                            checkTile.Add(board[kingTile.x - 1, kingTile.y - 1]);
                        }
                        else break;
                    }
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.bitshop)
                    {
                        board[x, y].piece.FindValidNextMove(board);
                        checkTile.Add(board[x, y]);
                        break;
                    }
                }
            }
            for (int x = kingTile.x - 1, y = kingTile.y + 1; x >= 0 && y <= 7; x--, y++) // left down
            {
                Piece piece = board[x, y].piece;
                if (piece != null)
                {
                    if (piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.pawn)
                    {
                        if (board[kingTile.x - 1, kingTile.y + 1].piece == piece)
                        {
                            checkTile.Add(board[kingTile.x - 1, kingTile.y + 1]);
                        }
                        else break;
                    }
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.bitshop)
                    {
                        board[x, y].piece.FindValidNextMove(board);
                        checkTile.Add(board[x, y]);
                        break;
                    }
                }
            }
            for (int x = kingTile.x + 1, y = kingTile.y + 1; x <= 7 && y <= 7; x++, y++) // right down
            {
                Piece piece = board[x, y].piece;
                if (piece != null)
                {
                    if (piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.pawn)
                    {
                        if (board[kingTile.x + 1, kingTile.y + 1].piece == piece)
                        {
                            checkTile.Add(board[kingTile.x + 1, kingTile.y + 1]);
                        }
                        else break;
                    }
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.bitshop)
                    {
                        board[x, y].piece.FindValidNextMove(board);
                        checkTile.Add(board[x, y]);
                        break;
                    }
                }
            }
            return checkTile;
        }

        private static List<Tile> ExploreKingHorVertical(Tile[,] board, Tile kingTile)
        {
            List<Tile> checkTile = new List<Tile>(4);
            for (int x = kingTile.x + 1; x <= 7; x++) // check right horizontal
            {
                Piece piece = board[x, kingTile.y].piece;
                if (piece != null)
                {
                    if (piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.rook)
                    {
                        board[x, kingTile.y].piece.FindValidNextMove(board);
                        checkTile.Add(board[x, kingTile.y]);
                        break;
                    }
                    else break;
                }
            }
            for (int x = kingTile.x - 1; x >=0 ; x--) // check left horizontal
            {
                Piece piece = board[x, kingTile.y].piece;
                if (piece != null)
                {
                    if (piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.rook)
                    {
                        board[x, kingTile.y].piece.FindValidNextMove(board);
                        checkTile.Add(board[x, kingTile.y]);
                        break;
                    }
                    else break;
                }
            }
            for (int y = kingTile.y + 1; y <= 7; y++) // check down vertical
            {
                Piece piece = board[kingTile.x, y].piece;
                if (piece != null)
                {
                    if (piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.rook)
                    {
                        board[kingTile.x, y].piece.FindValidNextMove(board);
                        checkTile.Add(board[kingTile.x, y]);
                        break;
                    }
                    else break;
                }
            }
            for (int y = kingTile.y - 1; y >= 0; y--) // check up vertical
            {
                Piece piece = board[kingTile.x, y].piece;
                if (piece != null)
                {
                    if (piece.colour == kingTile.piece.colour) break; // if is teamate piece
                    else if (piece.pieceType == PieceType.queen || piece.pieceType == PieceType.rook)
                    {
                        board[kingTile.x, y].piece.FindValidNextMove(board);
                        checkTile.Add(board[kingTile.x, y]);
                        break;
                    }
                    else break;
                }
            }
            return checkTile;
        }

        private static List<Tile> ExploreKingKnight(Tile[,] board, Tile kingTile)
        {
            int x = kingTile.x;
            int y = kingTile.y;
            List<Tile> checkTile = new List<Tile>(8);
            if (IsValidPoistion(x - 2, y - 1)) // left up
            {
                Piece piece = board[x - 2, y - 1].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {
                    board[x - 2, y - 1].piece.FindValidNextMove(board);
                    checkTile.Add(board[x - 2, y - 1]);
                }
            }
            if (IsValidPoistion(x - 2, y + 1)) // left down
            {
                Piece piece = board[x - 2, y + 1].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {   
                    board[x - 2, y + 1].piece.FindValidNextMove(board);
                    checkTile.Add(board[x - 2, y + 1]);
                }
            }
            if (IsValidPoistion(x + 2, y - 1)) // right up 
            {
                Piece piece = board[x + 2, y - 1].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {
                    board[x + 2, y - 1].piece.FindValidNextMove(board);
                    checkTile.Add(board[x + 2, y - 1]);
                }
            }
            if (IsValidPoistion(x + 2, y + 1)) // right down
            {
                Piece piece = board[x + 2, y + 1].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {
                    board[x + 2, y + 1].piece.FindValidNextMove(board);
                    checkTile.Add(board[x + 2, y + 1]);
                }
            }
            if (IsValidPoistion(x - 1, y - 2)) // up left
            {
                Piece piece = board[x - 1, y - 2].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {
                    board[x - 1, y - 2].piece.FindValidNextMove(board);
                    checkTile.Add(board[x - 1, y - 2]);
                }
            }
            if (IsValidPoistion(x + 1, y - 2)) // up right
            {
                Piece piece = board[x + 1, y - 2].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {
                    board[x + 1, y - 2].piece.FindValidNextMove(board);
                    checkTile.Add(board[x + 1, y - 2]);
                }
            }
            if (IsValidPoistion(x - 1, y + 2)) // down left
            {
                Piece piece = board[x - 1, y + 2].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {
                    board[x - 1, y + 2].piece.FindValidNextMove(board);
                    checkTile.Add(board[x - 1, y + 2]);
                }
            }
            if (IsValidPoistion(x + 1, y + 2)) // down right
            {
                Piece piece = board[x + 1, y + 2].piece;
                if (piece != null && piece.colour != kingTile.piece.colour && piece.pieceType == PieceType.knight)
                {
                    board[x + 1, y + 2].piece.FindValidNextMove(board);
                    checkTile.Add(board[x + 1, y + 2]);
                }
            }
            return checkTile;
        }

        private protected static bool IsValidPoistion(int x, int y)
        {
            return x >= 0 && y >= 0 && x <= 7 && y <= 7;
        } // check is it a valid coordinate in the board

        private protected static Tile GetKingPoistion(Tile[,] board, PieceColour kingColour)
        {
            foreach (Tile tile in board)
            {
                if (tile.piece?.colour == kingColour && tile.piece?.pieceType == PieceType.king) return tile;
            }
            return null;
        }

        public virtual void CheckKing(Tile[,] board)
        {
            return;
        } // exe every after a piece move

        private protected virtual List<Tile> FindValidNextMove(Tile[,] board)
        {
            return null;
        } // can't access this method in king inner class
    }

    public enum PieceType
    {
        none,
        pawn,
        knight,
        rook,
        king,
        queen,
        bitshop
    }

    public enum PieceColour
    {
        white, 
        black
    }
}
