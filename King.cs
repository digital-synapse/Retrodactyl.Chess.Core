using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class King : Piece
    {
        public King(Board board, Square location, Player color)
        {
            this.board = board;
            this.location = location;
            this.player = color;
            this.type = PieceType.King;
            this.fen = color == Player.Black ? 'k' : 'K';
            this.unicode = color == Player.Black ? '\u265A' : '\u2654';
            inv = player == Player.White ? -1 : 1;
        }
        private int inv;

        public bool IsChecked()
        {
            int origin_x = location.x;
            int origin_y = location.y;
            int x, y;

            /* Check straight lines */
            for (x = origin_x + 1; x < 8; x++) {
                var p = board[origin_y, x];
                if (p != null) {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Rook || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }
            }
            for (x = origin_x - 1; x > -1; x--)
            {
                var p = board[origin_y, x];
                if (p != null)
                {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Rook || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }
            }
            for (y = origin_y + 1; y < 8; y++)
            {
                var p = board[y, origin_x];
                if (p != null)
                {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Rook || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }
            }
            for (y = origin_y - 1; y > -1; y--)
            {
                var p = board[y, origin_x];
                if (p != null)
                {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Rook || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }
            }


            /* Check diagonals */
            x = origin_x + 1;
            y = origin_y + 1;
            for (; x < 8 && y < 8; x++, y++)
            {
                var p = board[y,x];
                if (p != null)
                {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Bishop || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }
            }
            x = origin_x - 1;
            y = origin_y - 1;
            for (; x >= 0 && y >= 0; x--, y--)
            {
                var p = board[y, x];
                if (p != null)
                {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Bishop || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }

            }
            x = origin_x - 1;
            y = origin_y + 1;
            for (; x >= 0 && y < 8; x--, y++)
            {
                var p = board[y, x];
                if (p != null)
                {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Bishop || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }
            }
            x = origin_x + 1;
            y = origin_y - 1;
            for (; x < 8 && y >= 0; x++, y--)
            {
                var p = board[y, x];
                if (p != null)
                {
                    if (p.player != player)
                    {
                        if (p.type == PieceType.Bishop || p.type == PieceType.Queen) return true;
                    }
                    else break;
                }
            }

            /* Check pawns */
            int forward_y = origin_y + inv;
            if (forward_y > -1 && forward_y < 8)
            {
                int left_x = origin_x - 1;
                if (left_x > -1)
                {
                    var p = board[forward_y, left_x];
                    if (p != null && p.player != player && p.type == PieceType.Pawn) return true;
                }
                int right_x = origin_x + 1;
                if (right_x < 8)
                {
                    var p = board[forward_y, right_x];
                    if (p != null && p.player != player && p.type == PieceType.Pawn) return true;
                }
            }

            /* Check knights */
            var pos = new Square(origin_x + 1, origin_y + 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }
            pos = new Square(origin_x + 2, origin_y + 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }
            pos = new Square(origin_x - 2, origin_y + 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }
            pos = new Square(origin_x - 1, origin_y + 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }
            pos = new Square(origin_x + 2, origin_y - 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }
            pos = new Square(origin_x + 1, origin_y - 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }
            pos = new Square(origin_x - 2, origin_y - 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }
            pos = new Square(origin_x - 1, origin_y - 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p != null && p.player != player && p.type == PieceType.Knight) return true;
            }

            // check kings
            if (origin_x > 0)
            {
                var p = board[origin_y, origin_x - 1];
                if (p != null && p.type == PieceType.King) return true;
                if (origin_y > 0)
                {
                    p = board[origin_y - 1, origin_x - 1];
                    if (p != null && p.type == PieceType.King) return true;
                }
                if (origin_y < 7)
                {
                    p = board[origin_y + 1, origin_x - 1];
                    if (p != null && p.type == PieceType.King) return true;
                }
            }
            if (origin_x < 7)
            {
                var p = board[origin_y, origin_x + 1];
                if (p != null && p.type == PieceType.King) return true;
                if (origin_y > 0)
                {
                    p = board[origin_y - 1, origin_x + 1];
                    if (p != null && p.type == PieceType.King) return true;
                }
                if (origin_y < 7)
                {
                    p = board[origin_y + 1, origin_x + 1];
                    if (p != null && p.type == PieceType.King) return true;
                }
            }
            if (origin_y > 0)
            {
                var p = board[origin_y - 1, origin_x];
                if (p != null && p.type == PieceType.King) return true;
            }
            if (origin_y < 7)
            {
                var p = board[origin_y + 1, origin_x];
                if (p != null && p.type == PieceType.King) return true;

            }

            return false;
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IEnumerable<Move> GetMoves()
        {
            
            IPiece p;

            //s
            var pos = new Square(location.x, location.y + 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }

            //e
            pos = new Square(location.x + 1, location.y);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }

            //se
            pos = new Square(location.x+1, location.y + 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }

            //ne
            pos = new Square(location.x+1, location.y - 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }

            //sw
            pos = new Square(location.x-1, location.y + 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }

            //w
            pos = new Square(location.x - 1, location.y);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }

            // nw
            pos = new Square(location.x - 1, location.y-1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }

            // n
            pos = new Square(location.x, location.y-1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            
            // castle moves
            if (moveCount == 0)
            {
                if (board[new Square(location.x + 1, location.y)] == null
                 && board[new Square(location.x + 2, location.y)] == null)
                {
                    pos = new Square(location.x + 3, location.y);
                    var rook = board[pos];
                    if (rook != null && rook.moveCount == 0)
                    {
                        yield return new Move(this, location, pos);
                    }
                    
                }
                if (board[new Square(location.x - 1, location.y)] == null
                    && board[new Square(location.x - 2, location.y)] == null
                    && board[new Square(location.x - 3, location.y)] == null)
                {
                    pos = new Square(location.x - 4, location.y);
                    var rook = board[pos];
                    if (rook != null && rook.moveCount == 0)
                    {
                        yield return new Move(this, location, pos);
                    }
                }
            }
        }

        public override void GetMoves(FastList<Move> moves)
        {            
            IPiece p;

            //n
            var pos = new Square(location.x, location.y - 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            //s
            pos = new Square(location.x, location.y + 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            //e
            pos = new Square(location.x + 1, location.y);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            //w
            pos = new Square(location.x - 1, location.y);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            //se
            pos = new Square(location.x + 1, location.y + 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            //ne
            pos = new Square(location.x+1, location.y - 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            // sw
            pos = new Square(location.x - 1, location.y + 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            //nw
            pos = new Square(location.x - 1, location.y - 1);
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }

            

            // castle moves
            if (moveCount == 0)
            {
                if (board[new Square(location.x + 1, location.y)] == null
                 && board[new Square(location.x + 2, location.y)] == null)
                {
                    pos = new Square(location.x + 3, location.y);
                    var rook = board[pos];
                    if (rook != null && rook.moveCount == 0)
                    {
                        moves.Add(new Move(this, location, pos));
                    }

                }
                if (board[new Square(location.x - 1, location.y)] == null
                    && board[new Square(location.x - 2, location.y)] == null
                    && board[new Square(location.x - 3, location.y)] == null)
                {
                    pos = new Square(location.x - 4, location.y);
                    var rook = board[pos];
                    if (rook != null && rook.moveCount == 0)
                    {
                        moves.Add(new Move(this, location, pos));
                    }
                }
            }
        }
    }
}
