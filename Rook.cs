using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class Rook : Piece
    {
        public Rook(Board board, Square location, Player color)
        {
            this.board = board;
            this.location = location;
            this.player = color;
            this.type = PieceType.Rook;
            this.fen = color == Player.Black ? 'r' : 'R';
            this.unicode = color == Player.Black ? '\u265C' : '\u2656';
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
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
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
                if (p == null) moves.Add( new Move(this, location, pos));
                else
                {
                    if (p.player != player) moves.Add(new Move(this, location, pos, p));
                    break;
                }
            }
        }
    }
}
