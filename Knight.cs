using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class Knight : Piece
    {
        public Knight(Board board, Square location, Player color)
        {
            this.board = board;
            this.location = location;
            this.player = color;
            this.type = PieceType.Knight;
            this.fen = color == Player.Black ? 'n' : 'N';
            this.unicode = color == Player.Black ? '\u265E' : '\u2658';
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IEnumerable<Move> GetMoves()
        {
            var pos = new Square(location.x + 1, location.y + 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            pos = new Square(location.x + 2, location.y + 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            pos = new Square(location.x - 2, location.y + 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            pos = new Square(location.x - 1, location.y + 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            pos = new Square(location.x + 2, location.y - 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            pos = new Square(location.x + 1, location.y - 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            pos = new Square(location.x - 2, location.y - 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
            pos = new Square(location.x - 1, location.y - 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) yield return new Move(this, location, pos, p);
            }
        }

        public override void GetMoves(FastList<Move> moves)
        {
            var pos = new Square(location.x + 1, location.y + 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
            pos = new Square(location.x + 2, location.y + 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
            pos = new Square(location.x - 2, location.y + 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
            pos = new Square(location.x - 1, location.y + 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
            pos = new Square(location.x + 2, location.y - 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
            pos = new Square(location.x + 1, location.y - 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
            pos = new Square(location.x - 2, location.y - 1);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
            pos = new Square(location.x - 1, location.y - 2);
            if (pos.isValid)
            {
                var p = board[pos];
                if (p == null || p.player != player) moves.Add(new Move(this, location, pos, p));
            }
        }
    }
}
