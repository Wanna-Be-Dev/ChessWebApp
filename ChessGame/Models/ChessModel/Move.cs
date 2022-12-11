namespace ChessModel
{ 
    /// <summary>A class representing a chess move.</summary>
    public class Move
    {
        public int StartPosition { get; }
        public int EndPosition { get; }
        
        public Piece Piece { get; }
        
        public Piece EatenPiece { get; }

        /// <summary>Constructor for a move.</summary>
        /// <param name="startPosition">The position that the moved piece leave</param>
        /// <param name="endPosition">The position on which the moved piece gets</param>
        /// <param name="piece">The piece which moves</param>
        /// <param name="eatenPiece">The piece that is eaten. Can be of type <c>None</c> if nothing is eaten</param>
        /// <returns>A move which has be found by the minimax as the best one.</returns>
        public Move(int startPosition, int endPosition, Piece piece, Piece eatenPiece)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            Piece = piece;
            EatenPiece = eatenPiece;
        }

        /// <summary>Whether or not the move eat a piece on the board.</summary>
        /// <returns>A boolean that indicates if a piece was eaten during this move.</returns>
        public bool Eat => EatenPiece.Type != ChessType.None;

        public override string ToString()
        {
            return StartPosition + ", " + EndPosition + ", " + Piece.Type + ", " + EatenPiece.Type;
        }

        public override bool Equals(object? obj)
        {
            Move m = obj as Move;
            if (m == null)
                return false;
            return m.StartPosition == StartPosition && 
                   m.EndPosition == EndPosition && 
                   m.Piece.Type == Piece.Type &&
                   m.EatenPiece.Type == EatenPiece.Type;
        }
    }
}