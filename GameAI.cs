using Retrodactyl.Chess.Core;
using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Retrodactyl.Chess.Core
{
    public class GameAI
    {
        public ExtMove Search(Board game)
        {
            try
            {
                var result = minimax(5, game, game.CurrentPlayer);
                //var result = minimax_8(5, game, game.CurrentPlayer);
                //var result = minimax_8_iterative_deepening(8, game, game.CurrentPlayer);

                return result;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        private IEnumerable<Move> getShuffleValidMoves(Board p)
        {
            return p.GetMoves().Shuffle();
        }


        // Sets the value for each piece using standard piece value
        private readonly static Dictionary<PieceType, int> pieceValueMap = new Dictionary<PieceType, int>() {
            { PieceType.Pawn, 125 },
            { PieceType.Knight, 325 }, // knights are only slightly less valuable than bishop because bishops have greater range
            { PieceType.Bishop, 350 },
            { PieceType.Rook, 525 },
            { PieceType.Queen, 1000 },
            { PieceType.King, 10000 }
         };

        private readonly static Dictionary<PieceType, short> pieceRiskMap = new Dictionary<PieceType, short>() {
            { PieceType.Pawn, 125 / 5 },
            { PieceType.Knight, 340 / 5 },
            { PieceType.Bishop, 350 / 5 },
            { PieceType.Rook, 525 / 5 },
            { PieceType.Queen, 1000 / 5 },
            { PieceType.King, 10000 / 5 }
         };

        private static readonly short[] pawnPositionTable = new short[]
        {
             0,  0,  0,  0,  0,  0,  0,  0,
            50, 50, 50, 50, 50, 50, 50, 50,
            20, 20, 30, 40, 40, 30, 20, 20,
             5,  5, 10, 30, 30, 10,  5,  5,
             0,  0,  0, 25, 25,  0,  0,  0,
             5, -5,-10,  0,  0,-10, -5,  5,
             5, 10, 10,-30,-30, 10, 10,  5,
             0,  0,  0,  0,  0,  0,  0,  0
        };
        private static readonly short[] knightPositionTable = new short[]
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-30,-20,-30,-30,-20,-30,-50,
        };

        private static readonly short[] bishopPositionTable = new short[]
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-40,-10,-10,-40,-10,-20,
        };
        private static readonly short[] kingPositionTable = new short[]
        {
          -30, -40, -40, -50, -50, -40, -40, -30,
          -30, -40, -40, -50, -50, -40, -40, -30,
          -30, -40, -40, -50, -50, -40, -40, -30,
          -30, -40, -40, -50, -50, -40, -40, -30,
          -20, -30, -30, -40, -40, -30, -30, -20,
          -10, -20, -20, -20, -20, -20, -20, -10,
           20,  20,   0,   0,   0,   0,  20,  20,
           20,  30,  10,   0,   0,  10,  30,  20
        };

        private static readonly short[] kingEndGamePositionTable = new short[]
        {
            -50,-40,-30,-20,-20,-30,-40,-50,
            -30,-20,-10,  0,  0,-10,-20,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-30,  0,  0,  0,  0,-30,-30,
            -50,-30,-30,-30,-30,-30,-30,-50
        };

        /*
                private int evaluateBoard(Board board, Player currentPlayer, bool isEndgame = false)
                {
                    // make the scoring asymetrical so each player values their own pieces
                    // more than the opponents pieces (strongly discourage sacrificial trades)
                    // eg.
                    // if (isWhite) { b = -1; w = 1; }
                    // else { b = 1; w = -1; }
                    int b = currentPlayer;
                    int w = currentPlayer * -1;

                    int score = 0;
                    var bishopCount = new byte[2];
                    var rookCount = new byte[2];
                    int side;
                    int player;
                    int index;
                    int i = 0;

                    Square s;
                    foreach (var p in board.pieces)
                    {
                        //for (var y = 0; y < 8; y++)
                        //{
                        //    for (var x = 0; x < 8; x++)
                        //    {
                        //        int value = 0;
                        //        var s = new Square(x,y);
                        //        var p = board[s];
                        //        if (p != null)
                        int value = 0;
                        s = p.location;
                        {
                            var t = p.type;
                            if (p.player == Player.Black)
                            {
                                side = b;
                                player = 1;
                                index = 63 - i;
                            }
                            else
                            {
                                side = w;
                                player = 0;
                                index = i;
                            }
                            //var side = p.color == PieceColor.Black ? b : w;
                            //var player = p.color == PieceColor.White ? 0 : 1;
                            //var index = p.color == PieceColor.Black ? (byte)(63 - i) : i;
                            value += pieceValueMap[t];

                            if (t == PieceType.Pawn)
                            {
                                var file = s.file;
                                if (file == 0 || file == 7) value -= 15; // rook pawns worth 15% less because they can only attack one way
                                value += pawnPositionTable[index];       // add position bonus / penalties (favor pawn advance)                        
                                                                         //pawnFileCount[player,file]++;                // penalty for doubled pawn
                                                                         //if (pawnFileCount[player, file] > 1) value -= 15;
                            }

                            else if (t == PieceType.Knight)
                            {
                                value += knightPositionTable[index];     // add position bonus ( favor center of board )
                                if (isEndgame) value -= 10;              // small penalty for knights in the end game since they are less effective
                            }

                            else if (t == PieceType.Bishop)
                            {
                                value += bishopPositionTable[index];    // add position bonus ( favor center of board )
                                bishopCount[player]++;
                                if (bishopCount[player] > 1) value += 10; // bonus for having both bishops
                                if (isEndgame) value += 10;               // bishops endgame bonus for being more effective
                            }

                            else if (t == PieceType.Rook)
                            {
                                if (isEndgame) value += 10;             // in the endgame, rooks are worth more
                                rookCount[player]++;
                                if (rookCount[player] > 1) value += 10; // bonus for having both rooks
                            }

                            else if (t == PieceType.King)
                            {
                                // king position bonus
                                if (isEndgame) value += kingEndGamePositionTable[index];
                                else value += kingPositionTable[index];
                            }

                            else if (t == PieceType.Queen)
                            {
                                if (!isEndgame)
                                {
                                    var rank = s.rank;
                                    if (rank > 0 && rank < 7)
                                        value -= 50; // big penalty for eager queens
                                }
                            }

                            // give player a bonus for their own pieces
                            // slight asymmetry to discourage sacrificial trades
                            if (p.player == board.CurrentPlayer)
                                value += 10;

                            score += (value * side);
                        }
                        i++;
                        //    }
                        //}
                    }
                    return score;
                }
        */
        private int evaluateBoard(Board board, Player currentPlayer)
        {
            // make the scoring asymetrical so each player values their own pieces
            // more than the opponents pieces (strongly discourage sacrificial trades)
            // eg.
            // if (isWhite) { b = -1; w = 1; }
            // else { b = 1; w = -1; }
            int b = currentPlayer;
            int w = currentPlayer * -1;
            int score = 0;
            var bishopCount = new byte[2];
            var rookCount = new byte[2];
            int side;
            int player;
            int index;
            int i = 0;
            Square s;
            foreach (var p in board.pieces)
            {
                int value = 0;
                s = p.location;
                {
                    var t = p.type;
                    if (p.player == Player.Black)
                    {
                        side = b;
                        player = 1;
                        index = 63 - i;
                    }
                    else
                    {
                        side = w;
                        player = 0;
                        index = i;
                    }
                    value += pieceValueMap[t];

                    if (t == PieceType.Pawn)
                    {
                        var file = s.file;
                        if (file == 0 || file == 7) value -= 15; // rook pawns worth 15% less because they can only attack one way
                        value += pawnPositionTable[index];       // add position bonus / penalties (favor pawn advance)                        
                                                                 //pawnFileCount[player,file]++;                // penalty for doubled pawn
                                                                 //if (pawnFileCount[player, file] > 1) value -= 15;
                    }

                    else if (t == PieceType.Knight)
                    {
                        value += knightPositionTable[index];     // add position bonus ( favor center of board )
                    }

                    else if (t == PieceType.Bishop)
                    {
                        value += bishopPositionTable[index];    // add position bonus ( favor center of board )
                        bishopCount[player]++;
                        if (bishopCount[player] > 1) value += 10; // bonus for having both bishops
                    }

                    else if (t == PieceType.Rook)
                    {
                        rookCount[player]++;
                        if (rookCount[player] > 1) value += 10; // bonus for having both rooks
                    }

                    else if (t == PieceType.King)
                    {
                        value += kingPositionTable[index];  // king position bonus
                    }

                    else if (t == PieceType.Queen)
                    {
                        var rank = s.rank;
                        if (rank > 0 && rank < 7)
                            value -= 50; // big penalty for eager queens
                    }

                    if (p.player == board.CurrentPlayer)        // give player a bonus for their own pieces
                        value += 10;                            // slight asymmetry to discourage sacrificial trades

                    score += (value * side);
                }
                i++;
            }
            return score;
        }

        public ExtMove minimax(int depth, Board game, Player currentPlayer, int alpha = int.MinValue, int beta = int.MaxValue, bool isMaximizingPlayer = true)
        {
            // Base case: evaluate board
            if (depth == 0)
                return new ExtMove(evaluateBoard(game, currentPlayer));

            ExtMove best = new ExtMove();
            int checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            int bestMoveValue = checkmate;
            var moves = getShuffleValidMoves(game);
            foreach (var move in moves)
            {
#if DEBUG
                Debug.Assert(move.piece != null);
#endif
                game.Move(move);
                if (game.IsMate)
                {
                    best = new ExtMove(move, bestMoveValue);
                }
                else
                {
                    var result = minimax(depth - 1, game, currentPlayer, alpha, beta, !isMaximizingPlayer);
                    var value = result.score;
                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Undo();
                if (beta < alpha) { break; }
            }
            return best;
        }

        public ExtMove minimax_moves(IEnumerable<Move> moves, int depth, Board game, Player currentPlayer, int alpha = int.MinValue, int beta = int.MaxValue, bool isMaximizingPlayer = true)
        {
            // Base case: evaluate board
            if (depth == 0)
                return new ExtMove(evaluateBoard(game, currentPlayer));

            ExtMove best = new ExtMove();
            int checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            int bestMoveValue = checkmate;

            foreach (var move in moves)
            {
#if DEBUG
                Debug.Assert(move.piece != null);
#endif
                game.Move(move);
                if (game.IsMate)
                {
                    best = new ExtMove(move, bestMoveValue);
                }
                else
                {
                    var result = minimax(depth - 1, game, currentPlayer, alpha, beta, !isMaximizingPlayer);
                    var value = result.score;
                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Undo();
                if (beta < alpha) { break; }
            }
            return best;
        }

        public ExtMove minimax_8(int depth, Board game, Player currentPlayer, int alpha = int.MinValue, int beta = int.MaxValue, bool isMaximizingPlayer = true)
        {
            var validMoves = getShuffleValidMoves(game).ToList();
            var movesPerThread = (int)Math.Ceiling(validMoves.Count / 8.0f);
            if (movesPerThread == 0)
            {
                return new ExtMove();
            }
            var moveLists = validMoves.Section(movesPerThread);
            var best8 = moveLists.AsParallel().Select(moves =>
            {
                var clone = new Board(game);
                var cloneMoves = moves.Select(m => new Move(clone[m.from], m.from, m.to,
                    m.capture != null ? clone[m.capture.location] : null)).ToList();
                if (cloneMoves.Count == 0)
                    return new ExtMove();
                else
                    return minimax_moves(cloneMoves, depth, clone, currentPlayer);
            }).ToList();
            var best = best8.OrderByDescending(x => Math.Abs(x.score)).First();
            return new ExtMove(new Move(game[best.from], best.from, best.to,
                best.capture == null ? null : game[best.capture.location]), best.score);
        }

        //----------------------------------------------------------------------



        public ExtMove minimax_iterative_deepening(int depth, Board game, Player currentPlayer, int alpha, int beta, bool isMaximizingPlayer, Dictionary<int, List<ExtMove>> bestMovesFromPreviousIterations, int maxDepth)
        {
            // Base case: evaluate board
            if (depth == 0)
                return new ExtMove(evaluateBoard(game, currentPlayer));

            ExtMove best = new ExtMove();
            int checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            int bestMoveValue = checkmate;
            IEnumerable<Move> moves;

            var prev = bestMovesFromPreviousIterations[maxDepth - depth];
            if (prev.Count > 0)
                moves = prev.Select( x=> (Move)x);
            else
                moves = getShuffleValidMoves(game);
            
            foreach (var move in moves)
            {
#if DEBUG
                Debug.Assert(move.piece != null);
#endif
                game.Move(move);
                if (game.IsMate)
                {
                    best = new ExtMove(move, bestMoveValue);
                }
                else
                {
                    var result = minimax_iterative_deepening(depth - 1, game, currentPlayer, alpha, beta, !isMaximizingPlayer, bestMovesFromPreviousIterations, maxDepth);
                    var value = result.score;
                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Undo();
                if (beta < alpha) { break; }
            }
            //bestMovesFromPreviousIterations[maxDepth - depth].Add(best);
            return best;
        }

        public ExtMove minimax_moves_iterative_deepening(IEnumerable<Move> moves, int depth, Board game, Player currentPlayer, int alpha, int beta, bool isMaximizingPlayer, Dictionary<int, List<ExtMove>> bestMovesFromPreviousIterations, int maxDepth)
        {
            // Base case: evaluate board
            if (depth == 0)
                return new ExtMove(evaluateBoard(game, currentPlayer));

            ExtMove best = new ExtMove();
            int checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            int bestMoveValue = checkmate;

            foreach (var move in moves)
            {
#if DEBUG
                Debug.Assert(move.piece != null);
#endif
                game.Move(move);
                if (game.IsMate)
                {
                    best = new ExtMove(move, bestMoveValue);
                }
                else
                {
                    var result = minimax_iterative_deepening(depth - 1, game, currentPlayer, alpha, beta, !isMaximizingPlayer, bestMovesFromPreviousIterations, maxDepth);
                    var value = result.score;
                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            best = new ExtMove(move, value);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Undo();
                if (beta < alpha) { break; }
            }
            bestMovesFromPreviousIterations[maxDepth - depth].Add(best);
            return best;
        }

        public ExtMove minimax_8_iterative_deepening(int depth, Board game, Player currentPlayer, int alpha = int.MinValue, int beta = int.MaxValue, bool isMaximizingPlayer = true)
        {
            Dictionary<int, List<ExtMove>> bestMovesFromPreviousIterations = new Dictionary<int, List<ExtMove>>()
            {
                {0, new List<ExtMove>() },
                {1, new List<ExtMove>() },
                {2, new List<ExtMove>() },
                {3, new List<ExtMove>() },
                {4, new List<ExtMove>() },
                {5, new List<ExtMove>() },
                {6, new List<ExtMove>() },
                {7, new List<ExtMove>() },
                {8, new List<ExtMove>() },
                {9, new List<ExtMove>() },
                {10, new List<ExtMove>() },
            };

            var maxDepth = depth;
            ExtMove bestMove = default(ExtMove);
            var validMoves = getShuffleValidMoves(game).ToList();
            var movesPerThread = (int)Math.Ceiling(validMoves.Count / 8.0f);
            if (movesPerThread == 0)
            {
                return new ExtMove();
            }
            var moveLists = validMoves.Section(movesPerThread).ToList();
            List<ExtMove> best8 = null;

            for (var d = 1; d < maxDepth; d++)
            {
                best8 = moveLists.AsParallel().Select(moves =>
                {
                    var clone = new Board(game);
                    var cloneMoves = moves.Select(m => new Move(clone[m.from], m.from, m.to,
                        m.capture != null ? clone[m.capture.location] : null)).ToList();
                    if (cloneMoves.Count == 0)
                        return new ExtMove();
                    else
                        return minimax_moves_iterative_deepening(cloneMoves, depth, clone, currentPlayer, alpha,beta, isMaximizingPlayer, bestMovesFromPreviousIterations, maxDepth);
                }).ToList();
                var best = best8.OrderByDescending(x => Math.Abs(x.score)).First();
                bestMovesFromPreviousIterations[0].Add(best);
                bestMove = new ExtMove(new Move(game[best.from], best.from, best.to,
                    best.capture == null ? null : game[best.capture.location]), best.score);
                moveLists = best8.Select(x => (IEnumerable<Move>) new List<Move>() { x }).ToList();
            }
            return bestMove;
        }

        //----------------------------------------------------------------------
        /*
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int evaluateBoard(IPosition pos, bool isPlayerWhite = true)
    {
        int b;
        int w;
        int value = 0;
        var board = pos.Board;
        for (var i = 0; i < 64; i++)
        {
            var s = new Square(i);
            var p = board.PieceAt(s);
            var t = p.Type();
            if (t != PieceTypes.NoPieceType)
            {

                int pawnRankBonus;
                if (t == PieceTypes.Pawn)
                    pawnRankBonus = s.Rank.AsInt();
                else pawnRankBonus = 0;

                int materialValue = pieceValueMap[t] * p.PlayerScale;

                value += 
                    ( materialValue         // white pieces have a positive value
                    + pawnRankBonus);       // slightly favor advancing pawns
            }
        }
        //if (pos.IsMate)
        //{
        //    if (pos.SideToMove.IsWhite) value = 50000;
        //    else value = -50000;
        //}
        if (!isPlayerWhite) value *= -1;
        return value;
    }
}  
    public class GameAI
    {
#region old attempts
        private Random rnd = new Random();

        // Sets the value for each piece using standard piece value
        private Dictionary<PieceTypes, int> pieceValueMap = new Dictionary<PieceTypes,int>() {
            { PieceTypes.Pawn, 100 },
            { PieceTypes.Knight, 340 }, // knights are only slightly less valuable than bishop because bishops have greater range
            { PieceTypes.Bishop, 350 },
            { PieceTypes.Rook, 525 },
            { PieceTypes.Queen, 1000 },
            { PieceTypes.King, 10000 }            
         };

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int evaluateBoard(IBoard board, bool isWhite)
        {
            int b;
            int w;
            int value = 0;
            if (isWhite) { b = -1; w = 1; }
            else { b = 1; w = -1; }

            for (var i=0; i<64; i++)
            {
                var s = new Square(i);
                var p = board.PieceAt(s);
                var t = p.Type();
                if (t != PieceTypes.NoPieceType)
                {
                    if (p.IsBlack)
                    {
                        value += pieceValueMap[t] * b;
                        if (t == PieceTypes.Pawn) value += s.Rank.AsInt() * b; // slightly favor advancing pawns
                    }
                    if (p.IsWhite)
                    {
                        value += pieceValueMap[t] * w;
                        if (t == PieceTypes.Pawn) value += s.Rank.AsInt() * w; // slightly favor advancing pawns
                    }
                }
            }
            return value;
        }

        public struct MiniMaxResult
        {
            public MiniMaxResult(int value) { this.value = value; move = null; }
            public MiniMaxResult(int value, Move move) { this.value = value; this.move = move; }

            public int value;
            public Move? move;
        }

        public MiniMaxResult minimax(int depth, IGame game, bool isPlayerWhite,int alpha= int.MinValue,int beta= int.MaxValue,bool isMaximizingPlayer= true)
        {
            // Base case: evaluate board
            if (depth == 0)
            {
                var boardValue = evaluateBoard(game.Pos.Board, isPlayerWhite);
                return new MiniMaxResult()
                {
                    value = boardValue,
                    move = null
                };
            }

            MiniMaxResult best = new MiniMaxResult();
            var checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            var bestMoveValue = checkmate; 

            foreach (var move in game.Pos.GenerateMoves().Where(m => m.Move != Move.EmptyMove).Shuffle())
            {
                var saveState = game.Pos.GenerateFen();
                game.Pos.MakeMove(move, game.Pos.State);
                if (game.Pos.IsMate)
                {
                    best = new MiniMaxResult(bestMoveValue, move);
                }
                else
                {
                    var result = minimax(depth - 1, game, isPlayerWhite, alpha, beta, !isMaximizingPlayer);
                    var value = result.value;
                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Pos.SetFen(saveState); // undo

                if (beta < alpha) { break; }
            }
            return best;
        }

        public MiniMaxResult minimax_shallow(int depth,IGame game,bool isPlayerWhite,int alpha = int.MinValue,int beta = int.MaxValue,bool isMaximizingPlayer = true)
        {
            // Base case: evaluate board
            if (depth == 0)
            {
                var boardValue = evaluateBoard(game.Pos.Board, isPlayerWhite);
                return new MiniMaxResult()
                {
                    value = boardValue,
                    move = null
                };
            }

            MiniMaxResult best = new MiniMaxResult();

            var checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            var bestMoveValue = checkmate;

            foreach (var move in game.Pos.GenerateMoves().Where(m => m.Move != Move.EmptyMove).Shuffle())
            {
                var saveState = game.Pos.GenerateFen();
                game.Pos.MakeMove(move, game.Pos.State);
                if (game.Pos.IsMate)
                {
                    best = new MiniMaxResult(bestMoveValue, move);
                }
                else
                {
                    var result = minimax_shallow(depth - 1, game, isPlayerWhite, alpha, beta, !isMaximizingPlayer);
                    var value = result.value;
                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            // add depth to favor shallow branches
                            value += depth;
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            // add depth to favor shallow branches
                            value -= depth;
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Pos.SetFen(saveState); // undo

                if (beta < alpha) { break; }
            }
            return best;
        }

        public ExtMove[] minimax_score(int depth,IGame game,bool isPlayerWhite,int alpha = int.MinValue,int beta = int.MaxValue,bool isMaximizingPlayer = true)
        {
            var bestMoveValue = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;


            var moves = game.Pos.GenerateMoves().Where(m => m.Move != Move.EmptyMove).Select(x=>x.Move).ToArray();
            ExtMove[] extMoves = new ExtMove[moves.Length];
            for( var m = 0; m < moves.Length; m++)
            {
                var move = moves[m];
                var saveState = game.Pos.GenerateFen();
                game.Pos.MakeMove(move, game.Pos.State);
                if (game.Pos.IsMate) extMoves[m] = new ExtMove(move, bestMoveValue);
                else
                {
                    var result = minimax(depth - 1, game, isPlayerWhite, alpha, beta, !isMaximizingPlayer);
                    var initialValue = evaluateBoard(game.Pos.Board, isPlayerWhite);
                    var value = result.value + initialValue;
                    extMoves[m] = new ExtMove(move, value);
                }
                game.Pos.SetFen(saveState); // undo
            }
            return extMoves;
        }


        public int pruned = 0;
        public MiniMaxResult minimax_sort(int depth,IGame game,bool isPlayerWhite,IEnumerable<ExtMove> moves,int alpha = int.MinValue,int beta = int.MaxValue,bool isMaximizingPlayer = true)
        {
            // Base case: evaluate board
            if (depth == 0)
            {
                var boardValue = evaluateBoard(game.Pos.Board, isPlayerWhite);
                return new MiniMaxResult()
                {
                    value = boardValue,
                    move = null
                };
            }

            MiniMaxResult best = new MiniMaxResult();
            var checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            var bestMoveValue = checkmate;

            var shallowScoredMoves = (moves != null ? moves : game.Pos.GenerateMoves())
                .Where(m => m.Move != Move.EmptyMove)
                .Shuffle()
                .OrderByDescending(m => m.Score);

            foreach (var move in shallowScoredMoves)
            {
                var saveState = game.Pos.GenerateFen();
                game.Pos.MakeMove(move, game.Pos.State);
                if (game.Pos.IsMate)
                {
                    best = new MiniMaxResult(checkmate, move);
                }
                else
                {
                    var result = minimax_sort(depth - 1, game, isPlayerWhite, null, alpha, beta, !isMaximizingPlayer);
                    var value = result.value;
                    if (isMaximizingPlayer)
                    {
                        // add depth to favor shallow branches
                        value += depth;
                        if (value > bestMoveValue)
                        {
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value; 
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        // sub depth to favor shallow branches
                        value -= depth;
                        if (value < bestMoveValue)
                        {
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Pos.SetFen(saveState); // undo

                if (beta < alpha) {
                    pruned++;
                    break; 
                }
            }
            return best;
        }

        public SearchResult BestMoveMiniMaxIterativeDeepening(IGame game, int maxTimeMillis = 10000, int maxDepth = 10)
        {
            const int iterativeDepth = 2;

            // clean up past moves that are no longer useful for the search cache
            int ply = game.Pos.Ply;
            //foreach (var key in scoreCache.Keys.Where(x => x < ply)) scoreCache.Remove(key);
            foreach (var key in moveCache.Keys.Where(x => x < ply)) moveCache.Remove(key);
            for (var p = ply; p< ply+maxDepth; p++)
            {
                //if (!scoreCache.ContainsKey(p)) scoreCache[p] = new Dictionary<string, ExtMove>();
                if (!moveCache.ContainsKey(p)) moveCache[p] = new Dictionary<string, List<ExtMove>>();
            }

            var player = game.CurrentPlayer().IsWhite;
            MiniMaxResult best = new MiniMaxResult();
            TimeSpan maxTime = TimeSpan.FromMilliseconds(maxTimeMillis);

            /*
            var sw = new Stopwatch();
            sw.Start();
            for (var d = 0; d < maxDepth-iterativeDepth; d++)
            {
                // shuffle move order
                if (moveCache[ply].ContainsKey(string.Empty))
                {
                    moveCache[ply][string.Empty] = moveCache[ply][string.Empty].Shuffle().ToList();
                }

                var result = minimax_iterative_deepening(d + iterativeDepth, game, player, ply: ply);

                if (Math.Abs(result.value) > Math.Abs(best.value))
                {
                    sw.Stop();
                    Debugger.Break();
                    sw.Start();
                    best = result;
                }

                var elapsed = sw.Elapsed;
                if (elapsed > maxTime)
                {
                    return new SearchResult
                    {
                        Move = best.move.Value,
                        Score = best.value,
                        Depth = d+ iterativeDepth,
                        Elapsed = elapsed,
                        Iterations = d
                    };
                }
            }

            return new SearchResult
            {
                Move = best.move.Value,
                Score = best.value,
                Depth = maxDepth,
                Elapsed = sw.Elapsed,
                Iterations = maxDepth - iterativeDepth
            };

            var depth = 5 + (ply / 2);
            if (depth > 7) depth = 7;
            var result = minimax_iterative_deepening(depth, game, player, ply: ply);
            return new SearchResult()
            {
                Move = result.move.Value,
                Score = result.value,
                Depth = depth,
            };
        }
        public struct SearchResult
        {
            public int Score { get; set; }
            public Move Move { get; set; }
            public int Depth { get; set; }
            public TimeSpan Elapsed { get; set; }
            public int Iterations { get; set; }
        }
        private static Dictionary<int,Dictionary<string, ExtMove>> scoreCache = new Dictionary<int, Dictionary<string, ExtMove>>();
        private static Dictionary<int,Dictionary<string, List<ExtMove>>> moveCache = new Dictionary<int, Dictionary<string, List<ExtMove>>>();
        private MiniMaxResult minimax_iterative_deepening(int depth,IGame game,bool isPlayerWhite,int alpha = int.MinValue,int beta = int.MaxValue,bool isMaximizingPlayer = true,string path = "",int ply =0)
        {
            // Base case: evaluate board
            if (depth == 0)
            {
                var boardValue = evaluateBoard(game.Pos.Board, isPlayerWhite);
                return new MiniMaxResult()
                {
                    value = boardValue,
                    move = null
                };
            }

            MiniMaxResult best = new MiniMaxResult();
            var checkmate = isMaximizingPlayer
              ? int.MinValue
              : int.MaxValue;
            var bestMoveValue = checkmate;

            IEnumerable<ExtMove> moves;
            List<ExtMove> scoredMoves;

            if (moveCache[ply].ContainsKey(path) && moveCache[ply][path].Count > 0)
            {
                moves = moveCache[ply][path];//.Shuffle();
                scoredMoves = null;
            }
            else
            {
                moves = game.Pos.GenerateMoves()
                    .Where(m => m.Move != Move.EmptyMove)
                    .Shuffle();
                scoredMoves = new List<ExtMove>();
            }


            foreach (var move in moves)
            {
                var saveState = game.Pos.GenerateFen();
                game.Pos.MakeMove(move, game.Pos.State);
                if (game.Pos.IsMate)
                {
                    best = new MiniMaxResult(checkmate, move);
                }
                else
                {
                    var moveString = move.Move.ToString();
                    var newPath = path + moveString;

                    int value;
                    //if (scoreCache[ply].ContainsKey(newPath))
                    //{
                    //   value = scoreCache[ply][newPath].Score;
                    //}
                    //else { 
                        var result = minimax_iterative_deepening(depth - 1, game, isPlayerWhite, alpha, beta, !isMaximizingPlayer, newPath, ply+1);
                        value = result.value;
                    //    var scoredMove = new ExtMove(move, value);
                    //    scoreCache[ply][newPath] = scoredMove;
                    //    if (scoredMoves != null)
                    //    {
                    //        scoredMoves.Add(scoredMove);
                    //    }
                    //}

                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            best = new MiniMaxResult(value, move);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Pos.SetFen(saveState); // undo

                if (beta < alpha)
                {
                    pruned++;
                    break;
                }
            }

            if (scoredMoves != null)
            {

                //if (depth > 1)
                //{
                    scoredMoves = scoredMoves.OrderByDescending(x => x.Score).ToList();
                //}
                //else
                //{
                //    scoredMoves = scoredMoves.OrderBy(x => x.Score).ToList();
                //}
                moveCache[ply][path] = scoredMoves; 
            }

            return best;
        }
#endregion

        public class DepthFirstSearchResult
        {
            public string Path;
            public Move Move;
            public long Score;            
        }

        public DepthFirstSearchResult DepthFirstSearch(IGame game, int maxDepth)
        {
            DepthFirstSearchResult best = new DepthFirstSearchResult();
            var history = new Dictionary<string, List<ExtMove>>();
            var path = string.Empty;
            var startingPly = game.Pos.Ply;
            var fen = game.Pos.GenerateFen().ToString();
            var nextFen = fen;
            var searching = true;
            string moveStr = null;
            var maximizeWhite = game.CurrentPlayer().IsWhite;
            try
            {
                while (searching)
                {
                    int bestMoveValue = 0;
                    int alpha = 0;
                    int beta = 0;
                    for (var ply = 0; ply < maxDepth; ply++)
                    {
                        var turn = ply % 2 == 0;
                        var moves = history.ContainsKey(path) ? history[path] 
                            : history[path] = scoreMovesCached(game, nextFen);
                        var move = moves.FirstOrDefault(m => !history.ContainsKey($"{path}/{m.Move}"));
                        if (move == default && ply < 2) move = moves[ rnd.Next(0, moves.Count)];
                        if (move != default)
                        {
                            var absScore = Math.Abs(move.Score);
                            if (absScore > bestMoveValue)
                            {
                                bestMoveValue = absScore;
                            }
                            game.Pos.MakeMove(move, game.Pos.State);
                            path = $"{path}/{move.Move}";
                            nextFen = game.Pos.GenerateFen().ToString();
                        }
                        else
                        {
                            searching = false;
                        }
                    }
                    game.Pos.SetFen(new FenData(fen));

                    if (bestMoveValue > best.Score)
                    {
                        best = new DepthFirstSearchResult()
                        {
                            Path = path,
                            Score = bestMoveValue
                        };
                    }
                    path = string.Empty;
                }

                moveStr = best.Path.Split('/',StringSplitOptions.RemoveEmptyEntries)[0];
                best.Move = moveStr;
                return best;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return default;
            }
        }

        private List<ExtMove> scoreMovesCached(IGame game, string fen, int fenId)
        {
            List<ExtMove> moves;
            if (!_scoreMovesCached.TryGetValue(fen, out moves))
            {
                moves = scoreMoves(game, fen, fenId);
                _scoreMovesCached[fen] = moves;
            }
            return moves;
        }
        private Dictionary<string, List<ExtMove>> _scoreMovesCached = new Dictionary<string, List<ExtMove>>();
        private List<ExtMove> scoreMoves(IGame game, string fen, int fenId)
        {
            var turn = game.CurrentPlayer().IsBlack;
            var scored = new List<ExtMove>();
            var prevScore = evaluateBoardCached(game.Pos.Board, fen, fenId, turn);
            foreach (var move in generateMovesCached(game.Pos, fen, fenId))
            {
                game.Pos.MakeMove(move, game.Pos.State);
                var newFen = game.Pos.GenerateFen();
                var currentScore = evaluateBoardCached(game.Pos.Board, newFen.ToString(), newFen.GetHashCode(), !turn);
                var score = currentScore + prevScore;
                if (game.Pos.IsMate) 
                    score *= 5;
                else if (game.Pos.InCheck) 
                    score *= 2;
                else if (move.IsPromotionMove()) 
                    score *= 3;
                scored.Add(new ExtMove(move, score));
                game.Pos.SetFen(new FenData(fen));
            }
            scored = scored.OrderByDescending(m => m.Score).ToList();
            return scored;
        }
        private int evaluateBoardCached(IBoard board, string fen, int fenId, bool turn)
        {
            int score;
            if (!_evaluateBoardCached.TryGetValue(fenId, out score))
            {
                score = evaluateBoard(board, turn);
                _evaluateBoardCached[fenId] = score;
            }
            return score;
        }
        private Dictionary<int, int> _evaluateBoardCached = new Dictionary<int, int>();
        private List<Move> generateMovesCached(IPosition pos, string fen, int fenId)
        {
            List<Move> moves;
            if (!_generateMovesCached.TryGetValue(fenId, out moves))
            {
                moves = pos.GenerateMoves().Select(x => x.Move).Where(x => x != default).Shuffle().ToList();
                _generateMovesCached[fenId] = moves;
            }
            return moves;
        }
        private Dictionary<int, List<Move>> _generateMovesCached = new Dictionary<int, List<Move>>();


        public MiniMaxResult minimax_cache(string fen, int fenId, int depth, IGame game, bool isPlayerWhite, int alpha = int.MinValue, int beta = int.MaxValue, bool isMaximizingPlayer = true)
        {

            // Base case: evaluate board
            if (depth == 0)
            {
                var boardValue = evaluateBoardCached(game.Pos.Board, fen, fenId, isPlayerWhite);
                return new MiniMaxResult(boardValue);
            }

            MiniMaxResult best = new MiniMaxResult();
            var checkmate = isMaximizingPlayer ? int.MinValue : int.MaxValue;
            var bestMoveValue = checkmate;

            foreach (var move in generateMovesCached(game.Pos, fen, fenId))
            {
                game.Pos.MakeMove(move, game.Pos.State);
                var newFen = game.Pos.GenerateFen();
                var result = minimax_cache(newFen.ToString(), newFen.GetHashCode(),
                    depth - 1, game, isPlayerWhite, alpha, beta, !isMaximizingPlayer);
                var value = result.value;
                if (isMaximizingPlayer)
                {
                    if (value > bestMoveValue)
                    {
                        best = new MiniMaxResult(value, move);
                        bestMoveValue = value;
                    }
                    alpha = Math.Max(alpha, value);
                }
                else
                {
                    if (value < bestMoveValue)
                    {
                        best = new MiniMaxResult(value, move);
                        bestMoveValue = value;
                    }
                    beta = Math.Min(beta, value);
                }
                game.Pos.SetFen(new FenData(fen)); // undo

                if (beta < alpha) { break; }
            }
            return best;
        }




        private ExtMove scoreMove(IGame game, Move move, FenData fen, int prevScore)
        {
            game.Pos.MakeMove(move, game.Pos.State);
            var currentScore = evaluateBoard(game.Pos.Board, game.CurrentPlayer().IsWhite);
            var score = currentScore + prevScore;
            if (game.Pos.IsMate)
                score *= 5;
            else if (game.Pos.InCheck)
                score *= 2;
            else if (move.IsPromotionMove())
                score *= 3;
            return new ExtMove(move,score);
        }
        private IEnumerable<Move> generateMoves(IPosition pos) => pos.GenerateMoves().Select(x => x.Move).Where(x => x != default).Shuffle();

        public MiniMaxResult minimax_search(int depth, IGame game, bool isPlayerWhite, int alpha = int.MinValue, int beta = int.MaxValue, bool isMaximizingPlayer = true)
        {
            if (depth == 0) return default;
            MiniMaxResult best = new MiniMaxResult();
            var checkmate = isMaximizingPlayer ? int.MinValue : int.MaxValue;
            var bestMoveValue = checkmate;
            var fen = game.Pos.GenerateFen().ToString();
            int modifier = 0;

            foreach (var move in generateMoves(game.Pos))
            {
                game.Pos.MakeMove(move, game.Pos.State);
                if (game.Pos.IsMate)
                    modifier = checkmate;
                else if (move.IsPromotionMove())
                    modifier = isMaximizingPlayer ? -200 : 200;
                else
                    modifier = 0;

                var result = minimax_search(depth - 1, game, isPlayerWhite, alpha, beta, !isMaximizingPlayer);
                var value = result.value + modifier;

                if (isMaximizingPlayer)
                {
                    if (value > bestMoveValue)
                    {
                        best = new MiniMaxResult(value, move);
                        bestMoveValue = value;
                    }
                    alpha = Math.Max(alpha, value);
                }
                else
                {
                    if (value < bestMoveValue)
                    {
                        best = new MiniMaxResult(value, move);
                        bestMoveValue = value;
                    }
                    beta = Math.Min(beta, value);
                }
                game.Pos.SetFen(new FenData(fen)); // undo

                if (beta < alpha) { break; }
            }
            return best;
        }

        public struct minimax_inline_result
        {
            public minimax_inline_result(int value) { this.value = value; move = default; }
            public minimax_inline_result(int value, Move move) { this.value = value; this.move = move; }

            public int value;
            public Move move;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public minimax_inline_result minimax_inline(int depth, IGame game, bool isPlayerWhite, int alpha = int.MinValue, int beta = int.MaxValue, bool isMaximizingPlayer = true)
        {
            // Base case: evaluate board
            if (depth == 0) return new minimax_inline_result(evaluateBoard(game.Pos.Board, isPlayerWhite));
            minimax_inline_result best = default;
            var checkmate = isMaximizingPlayer ? int.MinValue : int.MaxValue;
            int bestMoveValue = checkmate;
            foreach (var move in game.Pos.GenerateMoves().Where(m => m.Move != Move.EmptyMove).Shuffle())
            {
                var saveState = game.Pos.GenerateFen();
                game.Pos.MakeMove(move, game.Pos.State);
                if (game.Pos.IsMate) return new minimax_inline_result(bestMoveValue, move);                
                else
                {
                    var result = minimax_inline(depth - 1, game, isPlayerWhite, alpha, beta, !isMaximizingPlayer);
                    var value = result.value;
                    if (isMaximizingPlayer)
                    {
                        if (value > bestMoveValue)
                        {
                            best = new minimax_inline_result(value, move);
                            bestMoveValue = value;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                    else
                    {
                        if (value < bestMoveValue)
                        {
                            best = new minimax_inline_result(value, move);
                            bestMoveValue = value;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                game.Pos.SetFen(saveState); // undo
                if (beta < alpha) { break; }
            }
            return best;
        }
    }
    */

        public Forest<Move> InitMoveCache(int depth, Board game)
        {
            moveCache = move_search(depth, game, game.CurrentPlayer);
            return moveCache;
        }
        private Forest<Move> moveCache;


        public Forest<Move> move_search(int depth, Board game, Player currentPlayer, Forest<Move> forest = null, Forest<Move>.Node node = null)
        {
            if (depth > 0)
            {
                if (forest == null)
                {
                    forest = new Forest<Move>();
                    node = forest.Root;
                }

                var moves = getShuffleValidMoves(game).ToList();
                foreach (var move in moves)
                {
                    game.Move(move);
                    var child = node.AddChild(move);
                    move_search(depth - 1, game, currentPlayer, forest, child);
                    game.Undo();
                }
            }
            return forest;
        }
    }
}
