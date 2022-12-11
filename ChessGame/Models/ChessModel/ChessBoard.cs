using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessModel
{
    /// <summary>A interface that defines every public method a chessboard needs
    /// in order to be usable by other classes.</summary>
    public interface IChessBoard
    {
        public ChessColor NextToPlay { get; set; }
        
        public int PromoteWhite { get; }
        public int PromoteBlack { get; }

        /// <summary>Promote a pawn.</summary>
        /// <param name="type">The type to which we want to promote</param>
        /// <param name="color">The color of the piece that needs to be promoted.</param>
        public void PromotePawn(ChessType type, ChessColor color);

        /// <summary>Test if the current position is checkmate.</summary>
        /// <returns><c>True</c> if it is. <c>False</c> otherwise.</returns>
        public bool IsCheckMate { get; }
        
        /// <summary>Test if the current position is a draw.</summary>
        /// <returns><c>True</c> if it is. <c>False</c> otherwise.</returns>
        public bool IsDraw();

        /// <summary>Get the score of the current position.</summary>
        /// <returns>A float representing the score. Positive score means white are better. Negative score means black are better.</returns>
        public float GetEvaluationScore();

        /// <summary>Initialize the board, place the pieces, reset flags. After that, a game can begin</summary>
        public void InitializeBoard();

        /// <summary>Get all the legal moves for a player.</summary>
        /// <param name="color">The color of the player.</param>
        /// <returns>A list of all legal moves for every piece on the board which match the given color.</returns>
        public List<Move> GetAllLegalMoves(ChessColor color);

        /// <summary>Unplay the last move.</summary>
        public void Unplay();

        /// <summary>Plays a move.</summary>
        /// <param name="move">The move which has to be played.</param>
        /// <param name="simulation">Indicates if this is a real move or just a simulation made by an AI. Default value is <c>false</c></param>
        public void Play(Move move, bool simulation = false);

        /// <summary>Get the moves that has to be played.</summary>
        /// <returns>A list of moves.</returns>
        public List<Move> GetMoves();
        
        /// <summary>Set the moves that has to be played.</summary>
        /// <param name="moves">A list of moves.</param>
        public void SetMoves(List<Move> moves);
        
        /// <summary>Get the possible moves which can be done by the piece which is at the given position on the board.</summary>
        /// <param name="position">A position on the board. Can be anything between 0 and 63</param>
        /// <returns>A list of possible moves.</returns>
        public List<Move> GetMoveFromPosition(int position);
    }
    
    /// <summary>A class representing a chess board.</summary>
    public class ChessBoard : IChessBoard
    {
        public static ChessBoard Instance { get; private set; }
        
        public int PromoteWhite { get; private set; }
        public int PromoteBlack { get; private set; }

        public bool WhiteLeftCastle { get; private set; }
        public bool WhiteRightCastle { get; private set; }
        public bool BlackLeftCastle { get; private set; }
        public bool BlackRightCastle { get; private set; }

        public bool BlackHasCastle { get; private set; }
        public bool WhiteHasCastle { get; private set; }

        private int BlackCount { get; set; }
        private int WhiteCount { get; set; }

        public ChessColor NextToPlay { get; set; }

        private readonly Stack<MoveInfo> _moveHistory;
        public Piece[] Board { get; }

        private List<Move> _legalMoves;

        public List<Move> GetMoves()
        {
            return _legalMoves;
        }

        public void SetMoves(List<Move> moves)
        {
            _legalMoves = moves;
        }
        
        /// <summary>Constructor for a chess board.</summary>
        /// <returns>A chess board initialized and ready to be played on.</returns>
        public ChessBoard()
        {
            Board = new Piece[64];
            _moveHistory = new Stack<MoveInfo>(50);
            Instance = this;
            _legalMoves = new List<Move>();
            InitializeBoard();
        }

        /// <summary>Get the last move played.</summary>
        /// <returns>A move if it exists, null otherwise.</returns>
        public Move LastMove => _moveHistory.Any() ? _moveHistory.Peek().Move : null;

        public void Play(Move move, bool simulation = false)
        {
            _moveHistory.Push(new MoveInfo(move, WhiteLeftCastle, WhiteRightCastle, BlackLeftCastle, BlackRightCastle,
                WhiteHasCastle, BlackHasCastle));
            var startPosition = move.StartPosition;
            var endPosition = move.EndPosition;
            UpdateCastle(move, simulation);
            TestPromotion(move, simulation);
            UpdateScore(move.EatenPiece.Position);
            if (move.Eat)
                Board[move.EatenPiece.Position] = new Piece(ChessColor.None, move.EatenPiece.Position, ChessType.None);
            Switch(startPosition, endPosition);
            NextToPlay = NextToPlay.Reverse();
        }


        public void Unplay()
        {
            var moveInfo = _moveHistory.Pop();
            var move = moveInfo.Move;
            var startPosition = move.StartPosition;
            var endPosition = move.EndPosition;
            Switch(startPosition, endPosition);
            if (move.Eat)
                Board[move.EatenPiece.Position] = move.EatenPiece;
            if (NextToPlay == ChessColor.White)
                BlackCount -= Board[endPosition].Value;
            else
                WhiteCount -= Board[endPosition].Value;

            switch (endPosition)
            {
                case 2 when startPosition == 4:
                    Switch(0, 3);
                    break;
                case 6 when startPosition == 4:
                    Switch(5, 7);
                    break;
                case 58 when startPosition == 60:
                    Switch(56, 59);
                    break;
                case 62 when startPosition == 60:
                    Switch(61, 63);
                    break;
            }

            WhiteLeftCastle = moveInfo.WhiteLeftCastle;
            WhiteRightCastle = moveInfo.WhiteRightCastle;
            BlackLeftCastle = moveInfo.BlackLeftCastle;
            BlackRightCastle = moveInfo.BlackRightCastle;
            WhiteHasCastle = moveInfo.WhiteHasCastle;
            BlackHasCastle = moveInfo.BlackHasCastle;
            NextToPlay = NextToPlay.Reverse();
        }

        /// <summary>Switch 2 pieces on the board.</summary>
        /// <param name="position1">The first position.</param>
        /// <param name="position2">The second position</param>
        private void Switch(int position1, int position2)
        {
            (Board[position1], Board[position2]) = (Board[position2], Board[position1]);
            Board[position1].MoveTo(position1);
            Board[position2].MoveTo(position2);
        }

        /// <summary>Get the piece present at the given position.</summary>
        /// <param name="position">The position.</param>
        /// <returns>A piece.</returns>
        public Piece GetPiece(int position)
        {
            return Board[position];
        }

        public List<Move> GetMoveFromPosition(int position)
        {
            var piece = Board[position];
            return piece.Color == NextToPlay ? piece.GetLegalMoves() : new List<Move>();
        }

        public float GetEvaluationScore()
        {
            float score = WhiteCount - BlackCount;
            if (IsThreatening(35, ChessColor.White) || IsThreatening(36, ChessColor.White)) score += 0.1f;
            if (IsThreatening(27, ChessColor.Black) || IsThreatening(28, ChessColor.Black)) score -= 0.1f;
            if (!WhiteLeftCastle && !WhiteRightCastle && !WhiteHasCastle) score -= 0.9f;
            if (!BlackLeftCastle && !BlackRightCastle && !BlackHasCastle) score += 0.9f;
            if (WhiteHasCastle) score += 0.9f;
            if (BlackHasCastle) score -= 0.9f;
            return score;
        }

        /// <summary>Updates the score considering the piece eaten at last move.</summary>
        /// <param name="position">The position of the last eaten piece.</param>
        private void UpdateScore(int position)
        {
            if (NextToPlay == ChessColor.White)
                WhiteCount += Board[position].Value;
            else
                BlackCount += Board[position].Value;
        }

        /// <summary>Verifies whether or not a pawn can be promoted.</summary>
        /// <param name="move">The last move played.</param>
        /// <param name="simulation">Whether or not this is done by an AI or if it is a real move</param>
        private void TestPromotion(Move move, bool simulation)
        {
            if (move.Piece.Color == ChessColor.Black && move.Piece.Type == ChessType.Pawn &&
                move.EndPosition / 8 == 0 ||
                move.Piece.Color == ChessColor.White && move.Piece.Type == ChessType.Pawn && move.EndPosition / 8 == 7)
            {
                PromotePawn(move, simulation);
            }
        }

        /// <summary>Promote a pawn.</summary>
        /// <param name="move">The last move played.</param>
        /// <param name="simulation">Do nothing if it is a simulation</param>
        private void PromotePawn(Move move, bool simulation)
        {
            if (!simulation)
            {
                if (NextToPlay == ChessColor.White)
                    PromoteWhite = move.EndPosition;
                else 
                    PromoteBlack = move.EndPosition;
            }
        }

        public void PromotePawn(ChessType type, ChessColor color)
        {
            if (color == ChessColor.White)
            {
                Board[PromoteWhite].Type = type;
                PromoteWhite = -1;
            }
            else
            {
                Board[PromoteBlack].Type = type;
                PromoteBlack = -1;
            }
              
        }

        /// <summary>Test if last move correspond to a castle move and update flags.</summary>
        /// <param name="move">The last move played.</param>
        /// <param name="simulation">Whether or not this is done by an AI or if it is a real move</param>
        private void UpdateCastle(Move move, bool simulation)
        {
            if (move.StartPosition == 4 && (WhiteLeftCastle || WhiteRightCastle))
            {
                if (WhiteLeftCastle && move.EndPosition == 2)
                {
                    if (!simulation)
                        _legalMoves.Add(new Move(0, 3, Board[0], Board[3]));
                    Switch(0, 3);
                    WhiteHasCastle = true;
                }

                if (WhiteRightCastle && move.EndPosition == 6)
                {
                    if (!simulation)
                        _legalMoves.Add(new Move(7, 5, Board[7], Board[5]));
                    Switch(5, 7);
                    WhiteHasCastle = true;
                }

                WhiteLeftCastle = WhiteRightCastle = false;
            }
            else if (move.StartPosition == 60 && (BlackLeftCastle || BlackRightCastle))
            {
                if (BlackLeftCastle && move.EndPosition == 58)
                {
                    if (!simulation)
                        _legalMoves.Add(new Move(56, 59, Board[56], Board[59]));
                    Switch(56, 59);
                    BlackHasCastle = true;
                }

                if (BlackRightCastle && move.EndPosition == 62)
                {
                    if (!simulation)
                        _legalMoves.Add(new Move(63, 61, Board[63], Board[61]));
                    Switch(61, 63);
                    BlackHasCastle = true;
                }

                BlackLeftCastle = BlackRightCastle = false;
            }
            else if (WhiteLeftCastle && move.StartPosition == 0)
                WhiteLeftCastle = false;
            else if (WhiteRightCastle && move.StartPosition == 7)
                WhiteRightCastle = false;
            else if (BlackLeftCastle && move.StartPosition == 56)
                BlackLeftCastle = false;
            else if (BlackRightCastle && move.StartPosition == 63)
                BlackRightCastle = false;
        }
        
        public List<Move> GetAllLegalMoves(ChessColor color)
        {
            return Board.Where(piece => piece.Color == color).SelectMany(piece => piece.GetLegalMoves()).ToList();
        }

        /// <summary>Test if the current position is check for the given color.</summary>
        /// <param name="color">The color of the player that we want to test.</param>
        /// <returns><c>True</c> if it is check. <c>False</c> otherwise</returns>
        internal bool IsCheck(ChessColor color)
        {
            var position = Array
                    .Find(Board, piece1 =>
                        piece1.Color == color && piece1.Type == ChessType.King).Position;
            
            return Array.Exists(Board, piece =>
                piece.Color == color.Reverse() && piece.GetPseudoMoves().Exists(move => move.EndPosition.Equals(position)));
        }

        /// <summary>Test if a square is threatened by the player of the given color.</summary>
        /// <param name="position">The position we want to test</param>
        /// <param name="color">The color of the player.</param>
        /// <returns><c>True</c> if the square is threatened. <c>False</c> otherwise.</returns>
        internal bool IsThreatening(int position, ChessColor color)
        {
            return Array.Exists(Board, piece =>
                piece.Color == color && piece.GetPseudoMoves()
                    .Exists(move => move.EndPosition == position));
        }

        public bool IsCheckMate => IsCheck(NextToPlay) &&
                                   !Array.Exists(Board,
                                       piece => piece.Color == NextToPlay && piece.GetLegalMoves().Any());

        public bool IsDraw()
        {
            return IsPat || InsufficientMaterial;
        }

        /// <summary>Test if the current position is a pat.</summary>
        /// <returns><c>True</c> if it is. <c>False</c> otherwise.</returns>
        private bool IsPat => !IsCheck(NextToPlay) &&
                              !Array.Exists(Board, piece => piece.Color == NextToPlay && piece.GetLegalMoves().Any());

        /// <summary>Test if the current position is a draw by insufficiency of material.</summary>
        /// <returns><c>True</c> if it is. <c>False</c> otherwise.</returns>
        private bool InsufficientMaterial
        {
            get
            {
                var x = Board.Where(piece => piece.Type != ChessType.None && piece.Type != ChessType.King).ToList();
                return !x.Any() ||
                       x.Count == 1 && (x[0].Type == ChessType.Bishop || x[0].Type == ChessType.Knight) ||
                       x.Count == 2 && x[0].Type == ChessType.Bishop && x[1].Type == ChessType.Bishop &&
                       x[0].Color == x[1].Color;
            }
        }

        /// <summary>Test if a square contains a piece.</summary>
        /// <param name="position">The position we want to test.</param>
        /// <returns><c>True</c> if it is. <c>False</c> otherwise..</returns>
        internal bool IsEmpty(int position)
        {
            return Board[position].Type == ChessType.None;
        }

        /// <summary>Get the color of the piece which is at the given position.</summary>
        /// <param name="position">The position on the board</param>
        /// <returns>A color. If the square is empty, it returns <c>ChessColor.None</c>.</returns>
        public ChessColor Color(int position)
        {
            return GetPiece(position).Color;
        }

        public void InitializeBoard()
        {
            NextToPlay = ChessColor.White;
            Board[0] = new Piece(ChessColor.White, 0, ChessType.Rook);
            Board[1] = new Piece(ChessColor.White, 1, ChessType.Knight);
            Board[2] = new Piece(ChessColor.White, 2, ChessType.Bishop);
            Board[3] = new Piece(ChessColor.White, 3, ChessType.Queen);
            Board[4] = new Piece(ChessColor.White, 4, ChessType.King);
            Board[5] = new Piece(ChessColor.White, 5, ChessType.Bishop);
            Board[6] = new Piece(ChessColor.White, 6, ChessType.Knight);
            Board[7] = new Piece(ChessColor.White, 7, ChessType.Rook);
            for (var i = 8; i < 16; i++)
            {
                Board[i] = new Piece(ChessColor.White, i, ChessType.Pawn);
            }

            for (var i = 16; i < 48; i++)
            {
                Board[i] = new Piece(ChessColor.None, i, ChessType.None);
            }

            for (var i = 48; i < 56; i++)
            {
                Board[i] = new Piece(ChessColor.Black, i, ChessType.Pawn);
            }

            Board[56] = new Piece(ChessColor.Black, 56, ChessType.Rook);
            Board[57] = new Piece(ChessColor.Black, 57, ChessType.Knight);
            Board[58] = new Piece(ChessColor.Black, 58, ChessType.Bishop);
            Board[59] = new Piece(ChessColor.Black, 59, ChessType.Queen);
            Board[60] = new Piece(ChessColor.Black, 60, ChessType.King);
            Board[61] = new Piece(ChessColor.Black, 61, ChessType.Bishop);
            Board[62] = new Piece(ChessColor.Black, 62, ChessType.Knight);
            Board[63] = new Piece(ChessColor.Black, 63, ChessType.Rook);

            WhiteLeftCastle = WhiteRightCastle = BlackRightCastle = BlackLeftCastle = true;
            WhiteHasCastle = BlackHasCastle = false;
            PromoteWhite = PromoteBlack = -1;
            WhiteCount = BlackCount = 0;
        }

        /// <summary>Get the color of the tile on the board.</summary>
        /// <param name="position">The position of the tile.</param>
        /// <returns>A ChessColor. Can't be <c>ChessColor.None</c> since a tile must be either white or black.</returns>
        internal ChessColor TileColor(int position)
        {
            return position / 8 % 2 == 0 ? position % 2 == 0 ? ChessColor.Black :
                ChessColor.White :
                position % 2 == 0 ? ChessColor.White : ChessColor.Black;
        }

        public override string ToString()
        {
            return Board.Aggregate("", (current, piece) => current + (piece + "\n"));
        }
    }
}