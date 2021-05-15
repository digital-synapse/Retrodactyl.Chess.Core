using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class State
    {
        public State()
        {
            //WhiteMoves = new FastList<Move>(218);
            //BlackMoves = new FastList<Move>(218);
            CurrentMoves = new FastList<Move>(218);
            Player = Player.None;
        }

        public int Ply { get; private set; }
        public bool PlayerInCheck { get; private set; }
        //public FastList<Move> WhiteMoves { get; private set; }
        //public FastList<Move> BlackMoves { get; private set; }
        public FastList<Move> CurrentMoves { get; private set; }
        public Move Move { get; set; }
        public Player Player { get; private set; }

        public State Init(int ply, King whiteKing, King blackKing, Func<Move, bool> moveEvadesCheck, Action<Player> getMoves)
        {
            Ply = ply;
            Player = ply % 2 == 0 ? Player.Black : Player.White;
            getMoves(Player);
            PlayerInCheck = Player == Player.White
                ? whiteKing.IsChecked()
                : blackKing.IsChecked();

            // if the current player is in check filter out those moves
            // which would allow him to remain in check
            if (PlayerInCheck)
            {
                CurrentMoves.Init(CurrentMoves.GetEnumerable().Where(moveEvadesCheck).ToArray());
            }
            return this;
        }

        /*
        public State Init(int ply, IEnumerable<Move> whiteMoves, IEnumerable<Move> blackMoves, IPiece whiteKing, IPiece blackKing, Func<Move,bool> moveEvadesCheck)
        {
            WhiteMoves.Init(whiteMoves);
            BlackMoves.Init(blackMoves);
            Ply = ply;
            Player = ply % 2 == 0 ? Player.Black : Player.White;
            CurrentMoves = Player == Player.White
                ? WhiteMoves
                : BlackMoves;
            PlayerInCheck = Player == Player.White
                ? BlackMoves.Any(m => m.capture == whiteKing)
                : WhiteMoves.Any(m => m.capture == blackKing);

            // if the current player is in check filter out those moves
            // which would allow him to remain in check
            if (PlayerInCheck)
            {
                CurrentMoves.Init(CurrentMoves.GetEnumerable().Where(moveEvadesCheck).ToArray());
            }
            return this;
        }
        

        public State Init(int ply, IPiece whiteKing, IPiece blackKing, Func<Move, bool> moveEvadesCheck)
        {
            Ply = ply;
            Player = ply % 2 == 0 ? Player.Black : Player.White;
            CurrentMoves = Player == Player.White
                ? WhiteMoves
                : BlackMoves;
            PlayerInCheck = Player == Player.White
                ? BlackMoves.Any(m => m.capture == whiteKing)
                : WhiteMoves.Any(m => m.capture == blackKing);

            // if the current player is in check filter out those moves
            // which would allow him to remain in check
            if (PlayerInCheck)
            {
                CurrentMoves.Init(CurrentMoves.GetEnumerable().Where(moveEvadesCheck).ToArray());
            }
            return this;
        }
        */
    }
}
