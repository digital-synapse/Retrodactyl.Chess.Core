using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class Queen : Piece
    {
        public Queen(Board board, Square location, Player color)
        {
            this.board = board;
            this.location = location;
            this.player = color;
            this.type = PieceType.Queen;
            this.fen = color == Player.Black ? 'q' : 'Q';
            this.unicode = color == Player.Black ? '\u265B' : '\u2655';
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IEnumerable<Move> GetMoves()
        {
            for (var x = location.x + 1; x < 8; x++)
            {
                var pos = new Square(x, location.y);
                var p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
            for (var x = location.x - 1; x >= 0; x--)
            {
                var pos = new Square(x, location.y);
                var p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
            for (var y = location.y + 1; y < 8; y++)
            {
                var pos = new Square(location.x, y);
                var p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
            for (var y = location.y - 1; y >= 0; y--)
            {
                var pos = new Square(location.x, y);
                var p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
                else
                {
                    if (p.player != player) yield return new Move(this, location, pos, p);
                    break;
                }
            }
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
                    var p = board[pos];
                    if (p == null) yield return new Move(this, location, pos);
                    else
                    {
                        if (p.player != player) yield return new Move(this, location, pos, p);
                        break;
                    }
                }
            }
        }

        public override void GetMoves(FastList<Move> moves)
        {
            for (var x = location.x + 1; x < 8; x++)
            {
                var pos = new Square(x, location.y);
                var p = board[pos];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add( new Move(this, location, pos, p));
                    break;
                }
            }
            for (var x = location.x - 1; x >= 0; x--)
            {
                var pos = new Square(x, location.y);
                var p = board[pos];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
                    break;
                }
            }
            for (var y = location.y + 1; y < 8; y++)
            {
                var pos = new Square(location.x, y);
                var p = board[pos];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
                    break;
                }
            }
            for (var y = location.y - 1; y >= 0; y--)
            {
                var pos = new Square(location.x, y);
                var p = board[pos];
                if (p == null) moves.Add(new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add( new Move(this, location, pos, p));
                    break;
                }
            }
            {
                var x = location.x + 1;
                var y = location.y + 1;
                for (; x < 8 && y < 8; x++, y++)
                {
                    var pos = new Square(x, y);
                    var p = board[pos];
                    if (p == null) moves.Add( new Move(this, location, pos));
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
                        if (p.player != player) moves.Add( new Move(this, location, pos, p));
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
                        if (p.player != player) moves.Add( new Move(this, location, pos, p));
                        break;
                    }
                }
                x = location.x + 1;
                y = location.y - 1;
                for (; x < 8 && y >= 0; x++, y--)
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
            }
        }
    }
}
