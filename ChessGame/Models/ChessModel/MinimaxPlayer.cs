using System;
using System.Linq;
using ChessModel;
using Random = System.Random;

namespace ChessGame.Models.Player
{
    /// <summary>A class representing a chess player who determines best moves using a minimax algorithm embedded by
    /// alpha-beta pruning.</summary>
    public class MinmaxPlayer
    {
        private readonly int _depth;
        private readonly IChessBoard _board;
        private Move _bestMove;
        private readonly Random _rand;


        public MinmaxPlayer(int depth, IChessBoard chessBoard)
        {
            _depth = depth;
            _board = chessBoard;
            _rand = new Random();
        }

        /// <summary>Get the move the AI wants to play.</summary>
        /// <returns>A move which has be found by the minimax as the best one.</returns>
        public Move GetDesiredMove()
        {
            Minimax(_depth, float.MinValue, float.MaxValue, _board.NextToPlay);
            return _bestMove;
        }
        
        /// <summary>Get the move the AI wants to play.</summary>
        /// <param name="color">The color of the player who has to play.</param>
        /// <returns>A move which has be found by the minimax as the best one.</returns>
        public Move GetDesiredMove(ChessColor color)
        {
            Minimax(_depth, float.MinValue, float.MaxValue, color);
            return _bestMove;
        }

        
        /// <summary>Find the best possible move in the current position.</summary>
        /// <param name="depth">The depth to which the algorithm will recursively go find the best move.</param>
        /// <param name="alpha">A float needed by the alpha-beta pruning..</param>
        /// <param name="beta">A float needed by the alpha-beta pruning.</param>
        /// <param name="color">The color of the player who has to play.</param>
        private float Minimax(int depth, float alpha, float beta, ChessColor color)
        {
            if (_board.IsCheckMate)
                return _board.NextToPlay == ChessColor.White ? -100 : 100;

            if (_board.IsDraw())
                return 0;

            if (depth == 0)
                return _board.GetEvaluationScore();


            float value;
            var moves = _board.GetAllLegalMoves(color).OrderBy(item => _rand.Next());
            if (color == ChessColor.White)
            {
                value = float.MinValue;
                foreach (var move in moves)
                {
                    _board.Play(move, true);
                    var newValue = Minimax(depth - 1, alpha, beta, color.Reverse());
                    _board.Unplay();
                    alpha = Math.Max(alpha, newValue);
                    if (newValue > value)
                    {
                        value = newValue;
                        if (depth == _depth) _bestMove = move;
                    }

                    if (alpha >= beta)
                        break;
                }
            }
            else
            {
                value = float.MaxValue;
                foreach (var move in moves)
                {
                    _board.Play(move, true);
                    var newValue = Minimax(depth - 1, alpha, beta, color.Reverse());
                    _board.Unplay();
                    if (newValue < value)
                    {
                        value = newValue;
                        if (depth == _depth) _bestMove = move;
                    }

                    beta = Math.Min(beta, newValue);

                    if (alpha >= beta)
                        break;
                }
            }

            return value;
        }
    }
}