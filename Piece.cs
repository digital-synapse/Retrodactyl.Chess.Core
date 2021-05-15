using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public abstract class Piece: IPiece
    {
        public static IPiece Factory(char fen, Board board, Square location)
        {
            switch (fen)
            {
                case 'p': return new Pawn(board, location, Player.Black);
                case 'r': return new Rook(board, location, Player.Black);
                case 'n': return new Knight(board, location, Player.Black);
                case 'b': return new Bishop(board, location, Player.Black);
                case 'k': return new King(board, location, Player.Black);
                case 'q': return new Queen(board, location, Player.Black);
                case 'P': return new Pawn(board, location, Player.White);
                case 'R': return new Rook(board, location, Player.White);
                case 'N': return new Knight(board, location, Player.White);
                case 'B': return new Bishop(board, location, Player.White);
                case 'K': return new King(board, location, Player.White);
                case 'Q': return new Queen(board, location, Player.White);
            }
            return null;
        }

        public Board board { get; protected set; }
        public Square location { get; set; }
        public bool captured { get; set; }
        public Player player { get; set; }
        public PieceType type { get; set; }
        public char fen { get; protected set; }
        public char unicode { get; protected set; }
        public int moveCount { get; set; }
        public abstract IEnumerable<Move> GetMoves();

        public abstract void GetMoves(FastList<Move> moves);

        public override string ToString()
        {
            return fen.ToString()+location.ToString();
        }

        public IPiece Clone(Board parentBoard)
        {
            var p = Factory(fen, parentBoard, location);
            p.captured = captured;
            p.moveCount = moveCount;
            return p;
        }
    }
    public interface IPiece
    {
        Board board { get; }
        Square location { get; set; }
        bool captured { get; set; }
        Player player { get; }
        PieceType type { get; }
        char fen { get; }
        char unicode { get; }
        int moveCount { get; set;  }
        IEnumerable<Move> GetMoves();
        void GetMoves(FastList<Move> moves);
        IPiece Clone(Board parentBoard);
    }

    public struct PieceType : IEquatable<int>
    {

        public const int None = 0;
        public const int Pawn = 1;
        public const int Knight = 2;
        public const int Bishop = 3;
        public const int Rook = 4;
        public const int Queen = 5;
        public const int King = 6;

        public PieceType( int value) { this.value = value; }

        public int value;

        public static implicit operator PieceType(int value)
        {
            return new PieceType(value);
        }

        public static implicit operator int(PieceType value)
        {
            return value.value;
        }

        public bool Equals(int other) => value == other;
    }

    public struct Player : IEquatable<int>
    {
        public const int None = 0;
        public const int White = -1;
        public const int Black = 1;
        public const int Both = 2;
        public const int Current = 3;

        public Player( int value)
        {
            this.value = value;
        }
        public int value;

        public static implicit operator Player(int value)
        {
            return new Player(value);
        }

        public static implicit operator int(Player value)
        {
            return value.value;
        }

        public bool Equals(int other) => value == other;
    }

    public struct Move
    {
        public Move(IPiece piece, Square from, Square to, IPiece capture = null)
        {
            this.piece = piece;
            this.from = from;
            this.to = to;
            this.capture = capture;
        }
        public IPiece piece;
        public Square from;
        public Square to;
        public IPiece capture;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Move m1, Move m2)
        {
            return m1.capture == m2.capture && m1.from == m2.from && m1.to == m2.from && m1.piece == m2.piece;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Move m1, Move m2)
        {
            return m1.capture != m2.capture || m1.from != m2.from || m1.to != m2.from || m1.piece != m2.piece;
        }

        public override string ToString()
        {
            if (capture != null)
                return $"{piece.fen}{from} > {piece.fen}{to} {capture.fen}";
            else
                return $"{piece.fen}{from} > {piece.fen}{to}"; 
        }
    }

    public struct ExtMove
    {
        public ExtMove(int score = 0)
        {
            this.piece = default;
            this.from = default;
            this.to = default;
            this.capture = default;
            this.score = score;
        }
        public ExtMove(Move move, int score = 0)
        {
            this.piece = move.piece;
            this.from = move.from;
            this.to = move.to;
            this.capture = move.capture;
            this.score = score;
        }
        public ExtMove(IPiece piece, Square from, Square to, IPiece capture = null, int score =0)
        {
            this.piece = piece;
            this.from = from;
            this.to = to;
            this.capture = capture;
            this.score = score;
        }
        public IPiece piece;
        public Square from;
        public Square to;
        public IPiece capture;
        public int score;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Move(ExtMove i) => new Move(i.piece,i.from,i.to,i.capture);

        public override string ToString()
        {
            if (capture != null)
                return $"{piece.fen}{from} > {piece.fen}{to} {score.ToString().PadLeft(5)} {capture.fen}";
            else
                return $"{piece.fen}{from} > {piece.fen}{to} {score.ToString().PadLeft(5)}";
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ExtMove m1, ExtMove m2)
        {
            return m1.capture == m2.capture && m1.from == m2.from && m1.to == m2.from && m1.piece == m2.piece && m1.score == m2.score;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ExtMove m1, ExtMove m2)
        {
            return m1.capture != m2.capture || m1.from != m2.from || m1.to != m2.from || m1.piece != m2.piece || m1.score != m2.score;
        }
    }

    public struct Square : IEquatable<Square>
    {
        public Square(int i)
        {
            this.x = i % 8;
            this.y = i / 8;
        }
        public Square(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x;
        public int y;
        public int file => x;
        public int rank => y;
        public bool isValid => x >= 0 && y >= 0 && x < 8 && y < 8;


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToInt() => (int)this;

        public byte ToByte() => (byte)((x << 3) | y);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Square(int i) => new Square(i);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Square s) => s.y * 8 + s.x;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Square operator +(Square s1, Square s2) => new Square(s1.x + s2.x, s1.y+s2.y);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Square operator -(Square s1, Square s2) => new Square(s1.x - s2.x, s1.y-s2.y);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => y*8+x;
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object o)
        {
            if (o == null) return false;
            try
            {
                var s = (Square)o;
                return s == this;
            }
            catch
            {
                return false;
            }
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Square other)
        {
            return x == other.x && y == other.y;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Square s1, Square s2)
        {
            return s1.x == s2.x && s1.y == s2.y;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Square s1, Square s2)
        {
            return s1.x != s2.x || s1.y != s2.y;
        }

        public override string ToString()
        {
            return $"{(char)(x + 97)}{Math.Abs((y+1)*-1)}";
        }
    }


    /*
    public struct Square : IEquatable<Square>
    {
        public Square(int x, int y, int i)
        {
            this.x = x;
            this.y = y;
            this.i = i;
        }
        public Square(int i)
        {
            this.x = i % 8;
            this.y = i / 8;
            this.i = i;
        }
        public Square(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.i = y * 8 + x;
        }
        public int x;
        public int y;
        public int i;
        public bool IsValid => x >= 0 && y >= 0 && x < 8 && y < 8;

        public bool Equals([AllowNull] Square other)
        {
            //return x == other.x && y == other.y;
            return i == other.i;
        }
        public static bool operator == (Square s1, Square s2){
            //return s1.x == s2.x && s1.y == s2.y;
            return s1.i == s2.i;
        }
        public static bool operator !=(Square s1, Square s2)
        {
            //return s1.x != s2.x || s1.y != s2.y;
            return s1.i != s2.i;
        }
    }
    */
}
