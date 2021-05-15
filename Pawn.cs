using Retrodactyl.Extensions.DotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Retrodactyl.Chess.Core
{
    public class Pawn : Piece
    {
        public Pawn(Board board, Square location, Player color)
        {
            this.board = board;
            this.location = location;
            this.player = color;
            this.type = PieceType.Pawn;
            this.fen = color == Player.Black ? 'p' : 'P';
            this.unicode = color == Player.Black ? '\u265F' : '\u2659';
            fifthRank = player == Player.Black ? 4 : 3;
            advance1 = player;
            advance2 = player * 2;

        }

        public override IEnumerable<Move> GetMoves()
        {
            Square pos;
            Square left = new Square(location.x-1, location.y);
            Square right = new Square(location.x + 1, location.y);
            Square capLeft;
            Square capRight;
            IPiece p;

            
            pos = new Square(location.x, location.y + player); //white -1, black +1
            capLeft = new Square(location.x - 1, location.y + player);
            capRight = new Square(location.x + 1, location.y + player);

            // first move (2 square) advance
            if (moveCount == 0)//(location.y == 6)
            {
                var pos2 = new Square(location.x, location.y + (player * 2));
                if (pos2.isValid)
                {
                    p = board[pos2];
                    if (p == null) yield return new Move(this, location, pos2);
                }
            }

            //advance
            if (pos.isValid)
            {
                p = board[pos];
                if (p == null) yield return new Move(this, location, pos);
            }
            // capture
            if (capLeft.isValid)
            {
                p = board[capLeft];
                if (p != null && p.player != player) yield return new Move(this, location, capLeft, p);
            }
            if (capRight.isValid)
            {
                p = board[capRight];
                if (p != null && p.player != player) yield return new Move(this, location, capRight, p);
            }
            //enpassant            
            if (left.isValid && moveCount ==1)
            {
                p = board[left];
                if (p != null && p.player != player && p.type == PieceType.Pawn && board[capLeft] == null)
                {
                    var enOppLeft = new Square(location.x - 1, location.y + (player * 2));
                    IPiece oppLeft = enOppLeft.isValid ? oppLeft = board[enOppLeft] : null;
                    if (oppLeft != null && oppLeft.type == PieceType.Pawn && oppLeft.player != player)
                    {

                        yield return new Move(this, location, capLeft, p);
                    }
                }
            }
            if (right.isValid && moveCount ==1)
            {
                p = board[right];
                if (p != null && p.player != player && p.type == PieceType.Pawn && board[capRight] == null)
                {
                    var enOppRight = new Square(location.x + 1, location.y + (player * 2));
                    IPiece oppRight = enOppRight.isValid ? oppRight = board[enOppRight] : null;
                    if (oppRight != null && oppRight.type == PieceType.Pawn && oppRight.player != player)
                    {

                        yield return new Move(this, location, capRight, p);
                    }
                }
            }
        }
        protected int fifthRank;
        protected int advance1;
        protected int advance2;

        public override void GetMoves(FastList<Move> moves)
        {
            IPiece p;
            int origin_x = location.x;
            int origin_y = location.y;
            int advance1_y = origin_y + advance1;
            int left_x = origin_x - 1;
            int right_x = origin_x + 1;

            if (origin_x>0 && origin_x<7)
            {
                //advance
                if (advance1_y > -1 && advance1_y < 8)
                {
                    p = board[advance1_y, origin_x];
                    if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance1_y)));

                    // capture
                    p = board[advance1_y, left_x];
                    if (p != null && p.player != player) moves.Add(new Move(this, location, p.location, p));
                    p = board[advance1_y, right_x];
                    if (p != null && p.player != player) moves.Add(new Move(this, location, p.location, p));

                    // first move (2 square) advance
                    if (moveCount == 0)
                    {
                        int advance2_y = origin_y + advance2;
                        if (advance2_y > -1 && advance2_y < 8)
                        {
                            p = board[advance2_y, origin_x];
                            if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance2_y)));
                        }
                    }

                    //enpassant          
                    else if (moveCount == 2 &&                        // ensure that the capturing pawn is on its fifth rank
                            origin_y == fifthRank)                      // and has previously performed a 2square advance
                    {
                        p = board[origin_y, left_x];
                        if (p != null && p.player != player && p.type == PieceType.Pawn && board[advance1_y, left_x] == null)
                        {
                            moves.Add(new Move(this, location, new Square(left_x, advance1_y), p));
                        }
                        p = board[origin_y, right_x];
                        if (p != null && p.player != player && p.type == PieceType.Pawn && board[advance1_y, right_x] == null)
                        {
                            moves.Add(new Move(this, location, new Square(right_x, advance1_y), p));
                        }
                    }
                }
            }
            else if (origin_x == 0)
            {
                //advance
                if (advance1_y > -1 && advance1_y < 8)
                {
                    p = board[advance1_y, origin_x];
                    if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance1_y)));

                    // capture
                    p = board[advance1_y, right_x];
                    if (p != null && p.player != player) moves.Add(new Move(this, location, p.location, p));

                    // first move (2 square) advance
                    if (moveCount == 0)
                    {
                        int advance2_y = origin_y + advance2;
                        if (advance2_y > -1 && advance2_y < 8)
                        {
                            p = board[advance2_y, origin_x];
                            if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance2_y)));
                        }
                    }

                    //enpassant          
                    else if (moveCount == 2 &&                        // ensure that the capturing pawn is on its fifth rank
                            origin_y == fifthRank)                      // and has previously performed a 2square advance
                    {
                        
                        p = board[origin_y, right_x];
                        if (p != null && p.player != player && p.type == PieceType.Pawn && board[advance1_y, right_x] == null)
                        {
                            moves.Add(new Move(this, location, new Square(right_x, advance1_y), p));
                        }
                        
                    }
                }
            }
            else// if (origin_x == 7)
            {
                //advance
                if (advance1_y > -1 && advance1_y < 8)
                {
                    p = board[advance1_y, origin_x];
                    if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance1_y)));

                    // capture
                    p = board[advance1_y, left_x];
                    if (p != null && p.player != player) moves.Add(new Move(this, location, p.location, p));

                    // first move (2 square) advance
                    if (moveCount == 0)
                    {
                        int advance2_y = origin_y + advance2;
                        if (advance2_y > -1 && advance2_y < 8)
                        {
                            p = board[advance2_y, origin_x];
                            if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance2_y)));
                        }
                    }

                    //enpassant          
                    else if (moveCount == 2 &&                        // ensure that the capturing pawn is on its fifth rank
                            origin_y == fifthRank)                      // and has previously performed a 2square advance
                    {
                        p = board[origin_y, left_x];
                        if (p != null && p.player != player && p.type == PieceType.Pawn && board[advance1_y, left_x] == null)
                        {
                            moves.Add(new Move(this, location, new Square(left_x, advance1_y), p));
                        }
                    }
                }
            }
            
        }

        /*
        public override void GetMoves(FastList<Move> moves)
        {
            IPiece p;
            int origin_x = location.x;
            int origin_y = location.y;
            int advance1_y = origin_y + advance1;
            int left_x = origin_x - 1;
            int right_x = origin_x + 1;

            //advance
            if (advance1_y > -1 && advance1_y < 8)
            {
                p = board[advance1_y, origin_x];
                if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance1_y)));

                // capture
                if (left_x > -1)
                {
                    p = board[advance1_y, left_x];
                    if (p != null && p.player != player) moves.Add(new Move(this, location, p.location, p));
                }
                if (right_x < 8)
                {
                    p = board[advance1_y, right_x];
                    if (p != null && p.player != player) moves.Add(new Move(this, location, p.location, p));
                }
            }

            // first move (2 square) advance
            if (moveCount == 0)
            {
                int advance2_y = origin_y + advance2;
                if (advance2_y > -1 && advance2_y < 8)
                {
                    p = board[advance2_y, origin_x];
                    if (p == null) moves.Add(new Move(this, location, new Square(origin_x, advance2_y)));
                }
            }

            //enpassant          
            else if (moveCount == 2 &&                        // ensure that the capturing pawn is on its fifth rank
                    origin_y==fifthRank)                      // and has previously performed a 2square advance
            {
                if (left_x > -1)
                {
                    p = board[origin_y, left_x];
                    if (p != null && p.player != player && p.type == PieceType.Pawn && board[advance1_y, left_x] == null)
                    {
                        moves.Add(new Move(this, location, new Square(left_x, advance1_y), p));
                    }
                }
                if (right_x < 8)
                {
                    p = board[origin_y, right_x];
                    if (p != null && p.player != player && p.type == PieceType.Pawn && board[advance1_y, right_x] == null)
                    {
                        moves.Add(new Move(this, location, new Square(right_x, advance1_y), p));
                    }
                }
            }
        }
        */
    }
}
