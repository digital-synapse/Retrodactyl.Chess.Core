using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class Board
    {
        public Board(bool init = true)
        {
            // state history
            history = new HistoryStack<State>(24, i=> new State());
            state = history.Next();
            data = new IPiece[64];

            if (init)
            {
                white = new List<IPiece>(new IPiece[16]);
                black = new List<IPiece>(new IPiece[16]);
                pieces = new List<IPiece>(new IPiece[32]);
                //captured = new List<IPiece>(32);

                pieces[0] = black[0] = this[0, 0] = new Rook(this, new Square(0, 0), Player.Black);
                pieces[1] = black[1] = this[0, 1] = new Knight(this, new Square(1, 0), Player.Black);
                pieces[2] = black[2] = this[0, 2] = new Bishop(this, new Square(2, 0), Player.Black);
                pieces[3] = black[3] = this[0, 3] = new Queen(this, new Square(3, 0), Player.Black);
                pieces[4] = black[4] = this[0, 4] = blackKing = new King(this, new Square(4, 0), Player.Black);
                pieces[5] = black[5] = this[0, 5] = new Bishop(this, new Square(5, 0), Player.Black);
                pieces[6] = black[6] = this[0, 6] = new Knight(this, new Square(6, 0), Player.Black);
                pieces[7] = black[7] = this[0, 7] = new Rook(this, new Square(7, 0), Player.Black);
                
                pieces[8] = black[8] = this[1, 0] = new Pawn(this, new Square(0, 1), Player.Black);
                pieces[9] = black[9] = this[1, 1] = new Pawn(this, new Square(1, 1), Player.Black);
                pieces[10] = black[10] = this[1, 2] = new Pawn(this, new Square(2, 1), Player.Black);
                pieces[11] = black[11] = this[1, 3] = new Pawn(this, new Square(3, 1), Player.Black);
                pieces[12] = black[12] = this[1, 4] = new Pawn(this, new Square(4, 1), Player.Black);
                pieces[13] = black[13] = this[1, 5] = new Pawn(this, new Square(5, 1), Player.Black);
                pieces[14] = black[14] = this[1, 6] = new Pawn(this, new Square(6, 1), Player.Black);
                pieces[15] = black[15] = this[1, 7] = new Pawn(this, new Square(7, 1), Player.Black);
                pieces[16] = white[0] = this[6, 0] = new Pawn(this, new Square(0, 6), Player.White);
                pieces[17] = white[1] = this[6, 1] = new Pawn(this, new Square(1, 6), Player.White);
                pieces[18] = white[2] = this[6, 2] = new Pawn(this, new Square(2, 6), Player.White);
                pieces[19] = white[3] = this[6, 3] = new Pawn(this, new Square(3, 6), Player.White);
                pieces[20] = white[4] = this[6, 4] = new Pawn(this, new Square(4, 6), Player.White);
                pieces[21] = white[5] = this[6, 5] = new Pawn(this, new Square(5, 6), Player.White);
                pieces[22] = white[6] = this[6, 6] = new Pawn(this, new Square(6, 6), Player.White);
                pieces[23] = white[7] = this[6, 7] = new Pawn(this, new Square(7, 6), Player.White);
                pieces[24] = white[8] = this[7, 0] = new Rook(this, new Square(0, 7), Player.White);
                pieces[25] = white[9] = this[7, 1] = new Knight(this, new Square(1, 7), Player.White);
                pieces[26] = white[10] = this[7, 2] = new Bishop(this, new Square(2, 7), Player.White);
                pieces[27] = white[11] = this[7, 3] = new Queen(this, new Square(3, 7), Player.White);
                pieces[28] = white[12] = this[7, 4] = whiteKing = new King(this, new Square(4, 7), Player.White);
                pieces[29] = white[13] = this[7, 5] = new Bishop(this, new Square(5, 7), Player.White);
                pieces[30] = white[14] = this[7, 6] = new Knight(this, new Square(6, 7), Player.White);
                pieces[31] = white[15] = this[7, 7] = new Rook(this, new Square(7, 7), Player.White);

                Start();
            }
            else
            {
                white = new List<IPiece>();
                black = new List<IPiece>();
                pieces = new List<IPiece>();
                //captured = new List<IPiece>();
            }
        }

        public Board(Board board)
        {
            // state history
            history = new HistoryStack<State>(24, i => new State());
            state = history.Next();
            data = new IPiece[64];
            white = new List<IPiece>();
            black = new List<IPiece>();
            pieces = new List<IPiece>();

            foreach (var p in board.black)
            {
                var clone = p.Clone(this);
                if (clone.type == PieceType.King) blackKing = (King)clone;
                black.Add(clone);
                pieces.Add(clone);
            }
            foreach (var p in board.white)
            {
                var clone = p.Clone(this);
                if (clone.type == PieceType.King) whiteKing = (King)clone;
                white.Add(clone);
                pieces.Add(clone);
            }
            foreach (var p in pieces)
            {
                this[p.location] = p;
            }

            incrementPly(board.ply);
        }
        public Board(string fen)
        {
            // state history
            history = new HistoryStack<State>(24, i => new State());
            state = history.Next();
            data = new IPiece[64];
            white = new List<IPiece>();
            black = new List<IPiece>();
            pieces = new List<IPiece>();
            //captured = new List<IPiece>(32);

            var t= fen.Split(' ');
            var rows = t[0].Split('/');
            for(var y=0; y < 8; y++)
            {
                int x = 0;
                foreach (var c in rows[y])
                {
                    if (char.IsDigit(c))
                    {
                        x += int.Parse(c.ToString());
                    }
                    else
                    {
                        var p = Piece.Factory(c, this, new Square(x, y));
                        pieces.Add(p);
                        this[p.location] = p;
                        if (p.player == Player.Black)
                        {
                            black.Add(p);
                            if (p.type == PieceType.King) blackKing = (King)p;
                        }
                        else
                        {
                            white.Add(p);
                            if (p.type == PieceType.King) whiteKing = (King)p;
                        }
                        x++;
                    }
                }                
            }
#if DEBUG
            Debug.Assert(white.Count + black.Count == pieces.Count);
            //Debug.Assert(pieces.Count + captured.Count == 32);
#endif
            var ply = int.Parse(t.Last());
            incrementPly(ply);
        }

        private IPiece[] data;
        private List<IPiece> white;
        private List<IPiece> black;
        //private List<IPiece> captured;
        public List<IPiece> pieces { get; private set; }
        private King whiteKing;
        private King blackKing;

        public IPiece AddPiece(PieceType type, Player player, Square position)
        {
            IPiece piece = null;
            switch (type)
            {
                case PieceType.Pawn: piece = new Pawn(this, position, player); break;
                case PieceType.Queen: piece = new Queen(this, position, player); break;
                case PieceType.King: piece = new King(this, position, player); break;
                case PieceType.Rook: piece = new Rook(this, position, player); break;
                case PieceType.Bishop: piece = new Bishop(this, position, player); break;
                case PieceType.Knight: piece = new Knight(this, position, player); break;
                default: throw new InvalidOperationException("Unknown Piece Type");
            }
            pieces.Add(piece);
            if (player == Player.White)
            {
                white.Add(piece);
                if (type == PieceType.King) whiteKing = (King)piece;
            }
            else
            {
                black.Add(piece);
                if (type == PieceType.King) blackKing = (King)piece;
            }
            data[position] = piece;
            return piece;
        }

        public IPiece this[int i]
        {
            get => data[i];
            set => data[i] = value;
        }

        public IPiece this[int y, int x]
        {
            get => data[y * 8 + x];
            set => data[y * 8 + x] = value;
        }

        public IPiece this[Square location]
        {
            get => data[location.y * 8 + location.x];
            set => data[location.y * 8 + location.x] = value;
        }

        private ulong getBits(Func<IPiece, ulong> filter)
        {
            ulong result = 0;
            foreach (var bit in data.Select(filter))
            {
                result = (result << 1) | bit;
            }
            return result;
        }
        //private ulong getBitsNotOccupied() => getBits(p => p == null ? 1UL : 0UL);
        //private ulong getBitsWhite() => getBits(p => p != null && p.player == Player.White ? 1UL : 0UL);
        //private ulong getBitsBlack() => getBits(p => p != null && p.player == Player.Black ? 1UL : 0UL);

        //public ulong bitsNotOccupied { get; private set; }
        //public ulong bitsWhite { get; private set; }
        //public ulong bitsBlack { get; private set; }


        public void Start()
        {
            incrementPly(1);
        }

        private void incrementPly(int delta)
        {
            ply += delta;

            if (delta > 0)
            {
                // keep a list of current moves each time the turn is incremented
                // this is to prevent multiple evaluations of the board
                state = history.Next();

                /*
                state.WhiteMoves.Clear();
                foreach (var p in white)
                {
                    p.GetMoves(state.WhiteMoves);
                }

                state.BlackMoves.Clear();
                foreach (var p in black)
                {
                    p.GetMoves(state.BlackMoves);
                }
                */

                state.Init(ply, whiteKing, blackKing, MoveEvadesCheck, turn => {
                    state.CurrentMoves.Clear();
                    if (turn == Player.White)
                    {
                        foreach (var p in white)
                            p.GetMoves(state.CurrentMoves);
                    }
                    else
                    {
                        foreach (var p in black)
                            p.GetMoves(state.CurrentMoves);
                    }
                    /*
#if DEBUG
                    foreach (var move in state.CurrentMoves.GetEnumerable())
                    {
                        var p = this[move.from];
                        Debug.Assert(p == move.piece);
                        Debug.Assert(p.location == move.from);
                    }
#endif
                    */
                });
                
                /*
                state.Init(ply, 
                    white.SelectMany(x => x.GetMoves()), 
                    black.SelectMany(x => x.GetMoves()),
                    whiteKing,
                    blackKing,
                    MoveEvadesCheck);
                */
                history.Push(state);
            }
            else
            {
                history.Pop();
                state = history.Peek();
            }
        }

        public Player CurrentPlayer => state.Player;
        public bool InRepetition => history.Count >= 6 && history.Peek(6).GroupBy(x => x).Count() < 6;
        public bool CurrentPlayerInCheck => state.PlayerInCheck;

        public bool IsMate => state.PlayerInCheck && !GetMoves().Any();
        //private bool getIsBlackInCheck() => GetMovesKingCaptures(Player.White).Any();
        //private bool getIsWhiteInCheck() => GetMovesKingCaptures(Player.Black).Any();
        //public bool isMate => currentPlayer == Player.White
        //    ? !whiteKing.GetMoves().Any() && getIsWhiteInCheck()
        //    : !blackKing.GetMoves().Any() && getIsBlackInCheck();
        //=> ply % 2 == 0 ? PieceColor.Black : PieceColor.White;
        public int ply { get; private set; }
        private HistoryStack<State> history;
        private State state;

        /*
        public IEnumerable<Move> GetAllMovesForAllPieces()
            => state.WhiteMoves.GetEnumerable().Concat(state.BlackMoves.GetEnumerable());

        public IEnumerable<Move> GetAllMovesForWhite()
            => state.WhiteMoves.GetEnumerable();

        public IEnumerable<Move> GetAllMovesForBlack()
            => state.BlackMoves.GetEnumerable();

        public IEnumerable<Move> GetAttackMovesCurrentPlayer()
            => state.CurrentMoves.GetEnumerable().Where(m=>m.capture != default);

        public IEnumerable<Move> GetChecksFromWhite()
            => state.WhiteMoves.GetEnumerable().Where(m => m.capture == blackKing);

        public IEnumerable<Move> GetChecksFromBlack()
            => state.BlackMoves.GetEnumerable().Where(m => m.capture == whiteKing);

        public IEnumerable<Move> GetAllMovesCurrentPlayer()
            => state.CurrentMoves.GetEnumerable();  

        public IEnumerable<Move> GetChecksFromCurrentPlayer()
            => state.CurrentMoves.GetEnumerable().Where(m => m.capture != null && m.capture.type == PieceType.King);
        */

        public bool MoveEvadesCheck(Move move)
        {
#if DEBUG
            var p = this[move.from];
            Debug.Assert(p == move.piece);
            Debug.Assert(p.location == move.from);
#endif
            var player = state.Player;
            if (move.piece.player == player)
            {
                var king = player == Player.White ? whiteKing : blackKing;
                domove(move);
                var valid = !king.IsChecked();
                undomove(move);
                return valid;
            }
            return true;
        }



        public IEnumerable<Move> GetMoves() => state.CurrentMoves.GetEnumerable();

        public IEnumerable<Move> GetAttackMoves() => state.CurrentMoves.GetEnumerable().Where(x => x.capture != null);

        /*
        public IEnumerable<Move> GetMoves()
        {
            return GetMoves(MoveType.All, Player.Current);
        }
        public IEnumerable<Move> GetMoves(MoveType ofType)
        {
            return GetMoves(ofType, Player.Current);
        }

        public IEnumerable<Move> GetMoves(MoveType ofType, Player getMovesFor)
        {

            if (state.PlayerInCheck)
                return getMovesInCheck(ofType, getMovesFor);   
            
            else
                return getMoves(ofType, getMovesFor);
        }

        private IEnumerable<Move> getMovesInCheck(MoveType ofType, Player getMovesFor)
        {
            foreach (var move in getMoves(ofType, getMovesFor))
            {
                _getMovesInCheck[moveCount] = move;
                moveCount++;
            }
            return _getMovesInCheck.Take(moveCount).Where(MoveEvadesCheck);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<Move> getMoves(MoveType ofType, Player getMovesFor)
        {
            switch (ofType)
            {
                case MoveType.All:
                    switch (getMovesFor)
                    {
                        case Player.Black: return black.SelectMany(x => x.GetMoves());
                        case Player.White: return white.SelectMany(x => x.GetMoves());
                        case Player.Both: return pieces.SelectMany(x => x.GetMoves());
                        case Player.Current:
                            return currentPlayer == Player.White
                               ? white.SelectMany(x => x.GetMoves())
                               : black.SelectMany(x => x.GetMoves());
                    }
                    break;
                case MoveType.Capture:
                    switch (getMovesFor)
                    {
                        case Player.Black: return black.SelectMany(x => x.GetMoves().Where(m => m.capture != null));
                        case Player.White: return white.SelectMany(x => x.GetMoves().Where(m => m.capture != null));
                        case Player.Both: return pieces.SelectMany(x => x.GetMoves().Where(m => m.capture != null));
                        case Player.Current:
                            return currentPlayer == Player.White
                               ? white.SelectMany(x => x.GetMoves().Where(m => m.capture != null))
                               : black.SelectMany(x => x.GetMoves().Where(m => m.capture != null));
                    }
                    break;
                case MoveType.Normal:
                    switch (getMovesFor)
                    {
                        case Player.Black: return black.SelectMany(x => x.GetMoves().Where(m => m.capture == null));
                        case Player.White: return white.SelectMany(x => x.GetMoves().Where(m => m.capture == null));
                        case Player.Both: return pieces.SelectMany(x => x.GetMoves().Where(m => m.capture == null));
                        case Player.Current:
                            return currentPlayer == Player.White
                               ? white.SelectMany(x => x.GetMoves().Where(m => m.capture == null))
                               : black.SelectMany(x => x.GetMoves().Where(m => m.capture == null));
                    }
                    break;
                case MoveType.Checks:
                    switch (getMovesFor)
                    {
                        case Player.Black: return black.SelectMany(x => x.GetMoves().Where(m => m.capture == whiteKing));
                        case Player.White: return white.SelectMany(x => x.GetMoves().Where(m => m.capture == blackKing));
                        case Player.Both: return pieces.SelectMany(x => x.GetMoves().Where(m => m.capture != null && m.capture.type == PieceType.King));
                        case Player.Current:
                            return currentPlayer == Player.White
                               ? white.SelectMany(x => x.GetMoves().Where(m => m.capture == blackKing))
                               : black.SelectMany(x => x.GetMoves().Where(m => m.capture == whiteKing));
                    }
                    break;
            }
            return null;
        }


        public IEnumerable<Move> GetMovesKingCaptures(Player getMovesFor)
        {
            switch (getMovesFor)
            {
                case Player.Black: return black.SelectMany(x => x.GetMoves()).Where(m => m.capture == whiteKing);
                case Player.White: return white.SelectMany(x => x.GetMoves()).Where(m => m.capture == blackKing);
                case Player.Both: return pieces.SelectMany(x => x.GetMoves()).Where(m => m.capture != null && m.capture.type == PieceType.King);
                case Player.Current:
                    return currentPlayer == Player.White
                        ? white.SelectMany(x => x.GetMoves()).Where(m => m.capture == blackKing)
                        : black.SelectMany(x => x.GetMoves()).Where(m => m.capture == whiteKing);
            }
            return null;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveEvadesCheck(Move move)
        {
            var player = currentPlayer;
            if (move.piece.player == player)
            {
                bool valid;
                Move(move);
                var inCheck = GetMovesKingCaptures(Player.Current).Any();
                valid = !inCheck;
                Undo();
                return valid;
            }
            return true;
        }
        */


        private void domove(Move move)
        {

#if DEBUG
            var p = this[move.from];
            Debug.Assert(p == move.piece);
            Debug.Assert(p.location == move.from);
#else
            var p = move.piece;
#endif

            //var p = move.piece;

            // handle capture first
            if (move.capture != null)
            {
                // todo: figure out why board cloning
                // is causing captured pieces escape
                // scope
                // this line shouldn't be needed!
                //move.capture = this[move.capture.location];

#if DEBUG
                var c = this[move.capture.location];
                Debug.Assert(c.Equals( move.capture));
                Debug.Assert(c.location.Equals( move.capture.location));
#endif
                move.capture.captured = true;
                this[move.capture.location] = null;
                
                pieces.Remove(move.capture);
                if (move.capture.player == Player.White)
                    white.Remove(move.capture);
                else
                    black.Remove(move.capture);

                //captured.Add(move.capture);
#if DEBUG
                Debug.Assert(white.Count + black.Count == pieces.Count);
                //Debug.Assert(pieces.Count + captured.Count == 32);
#endif
            }

            // special case for castling
            bool castle = false;
            if (move.piece.type == PieceType.King)
            {
                var rook = this[move.to];
                if (rook != null && rook.type == PieceType.Rook && rook.player == move.piece.player)
                {
                    var king = move.piece;
                    if (move.to.x > move.from.x)
                    {
                        this[rook.location] = null;
                        rook.location = new Square(5, move.from.y);
                        this[rook.location] = rook;
                        this[king.location] = null;
                        king.location = new Square(6, move.from.y);
                        this[king.location] = king;
                        castle = true;
                    }
                    else
                    {
                        this[rook.location] = null;
                        rook.location = new Square(3, move.from.y);
                        this[rook.location] = rook;
                        this[king.location] = null;
                        king.location = new Square(2, move.from.y);
                        this[king.location] = king;
                        castle = true;
                    }
                }
            }

            // move piece
            if (!castle)
            {
                p.location = move.to;
                this[move.from] = null;
                this[move.to] = p;
            }

            p.moveCount++;
        }

        public void Move(Move move)
        {
            domove(move);

            // state changes
            state.Move = move;
            incrementPly(1);
        }

        private void undomove(Move move)
        {
            move.piece.moveCount--;

            // undo castling left
            if (move.piece.type == PieceType.King && move.to.x == move.from.x - 4)
            {
                var rook = this[new Square(move.piece.location.x + 1, move.piece.location.y)];
                var king = move.piece;
                rook.location = new Square(0, move.from.y);
                this[new Square(3, move.from.y)] = null;
                this[rook.location] = rook;
                king.location = new Square(4, move.from.y);
                this[new Square(2, move.from.y)] = null;
                this[king.location] = king;
            }

            // undo castling right
            else if (move.piece.type == PieceType.King && move.to.x == move.from.x + 3)
            {
                var rook = this[new Square(move.piece.location.x - 1, move.piece.location.y)];
                var king = move.piece;
                rook.location = new Square(7, move.from.y);
                this[new Square(5, move.from.y)] = null;
                this[rook.location] = rook;
                king.location = new Square(4, move.from.y);
                this[new Square(6, move.from.y)] = null;
                this[king.location] = king;
            }

            // reverse move
            else
            {
                var p = move.piece;
                p.location = move.from;
                this[move.from] = p;
                this[move.to] = null;
            }

            // reverse capture
            if (move.capture != null)
            {
                move.capture.captured = false;
                this[move.capture.location] = move.capture;
                //captured.Remove(move.capture);
                pieces.Add(move.capture);
                if (move.capture.player == Player.White)
                    white.Add(move.capture);
                else
                    black.Add(move.capture);

            }
        }
        public void Undo()
        {
            incrementPly(-1);
            var move = state.Move;
            undomove(move);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int rle = 0;
            int x = 0;
            for (int i=0; i<64; i++)
            {
                x++;
                var p = data[i];
                if (p == null)
                {
                    rle++;
                }
                else {
                    if (rle > 0)
                    {
                        sb.Append(rle);
                        rle = 0;
                    }
                    sb.Append(p.fen);
                }
                if (x == 8 && i != 63)
                {
                    if (rle > 0)
                    {
                        sb.Append(rle);
                        rle = 0;
                    }
                    sb.Append('/');
                    x = 0;
                }
            }
            sb.Append(' ');
            if (CurrentPlayer == Player.Black) sb.Append('b');
            else sb.Append('w');
            sb.Append(' ');
            sb.Append(ply);
            return sb.ToString();
        }

        public string ToAscii()
        {
            var sb = new StringBuilder();
            for (var y=0; y<8; y++)
            {
                for (var x=0; x<8; x++)
                {
                    var p = this[y, x];
                    if (p == null)
                        sb.Append(" . ");
                    else
                        sb.Append($" {p.fen} ");
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        public string ToColorANSI()
        {
            var sb = new StringBuilder();
            var clear = "\u001b[0m";
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var bg = (x+y) % 2 == 0
                        ? "\u001b[48;5;244m"
                        : "\u001b[48;5;242m";
                    var p = this[y, x];
                    if (p == null)
                        sb.Append($"{bg}   {clear}");
                    else
                    {
                        var fg = p.player == Player.Black
                            ? "\u001b[38;5;0m"
                            : "\u001b[38;5;230m";

                        sb.Append($"{bg}{fg} {p.fen.ToString().ToUpper()} {clear}");
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }

    public enum MoveType
    {
        All,
        Capture,
        Normal,
        Checks
    }
}
