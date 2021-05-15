using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class Bishop : Piece
    {
        public Bishop(Board board, Square location, Player color)
        {
            this.board = board;
            this.location = location;
            this.player = color;
            this.type = PieceType.Bishop;
            this.fen = color == Player.Black ? 'b' : 'B';
            this.unicode = color == Player.Black ? '\u265D' : '\u2657';
        }

        public override IEnumerable<Move> GetMoves()
        {
            var x = location.x + 1;
            var y = location.y + 1;
            for (; x < 8 && y < 8; x++, y++)
            {
                var pos = new Square(x, y);
                var p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
            x = location.x - 1;
            y = location.y - 1;
            for (; x >= 0 && y >= 0; x--, y--)
            {
                var pos = new Square(x, y);
                var p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
            x = location.x - 1;
            y = location.y + 1;
            for (; x >= 0 && y < 8; x--, y++)
            {
                var pos = new Square(x, y);
                var p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
            x = location.x + 1;
            y = location.y - 1;
            for (; x < 8 && y >= 0; x++, y--)
            {
                var pos = new Square(x, y);
                var p = board[y, x];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
        }

        public override void GetMoves(FastList<Move> moves)
        {
            var x = location.x + 1;
            var y = location.y + 1;
            for (; x < 8 && y < 8; x++, y++)
            {
                var pos = new Square(x, y);
                var p = board[pos];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
                    break;
                }
            }
            x = location.x - 1;
            y = location.y - 1;
            for (; x >= 0 && y >= 0; x--, y--)
            {
                var pos = new Square(x, y);
                var p = board[pos];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
                    break;
                }
            }
            x = location.x - 1;
            y = location.y + 1;
            for (; x >= 0 && y < 8; x--, y++)
            {
                var pos = new Square(x, y);
                var p = board[pos];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
                    break;
                }
            }
            x = location.x + 1;
            y = location.y - 1;
            for (; x < 8 && y >= 0; x++, y--)
            {
                var pos = new Square(x, y);
                var p = board[y, x];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
                    break;
                }
            }
        }
    }
}
