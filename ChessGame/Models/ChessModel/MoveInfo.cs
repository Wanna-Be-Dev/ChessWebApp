namespace ChessModel
{
    /// <summary>A class containing every information needed in order to undo a move.
    /// It store a move and every castling flag</summary>
    public struct MoveInfo
    {
        public Move Move { get; }
        public bool WhiteLeftCastle { get; }
        public bool WhiteRightCastle { get; }
        public bool BlackLeftCastle { get; }
        public bool BlackRightCastle { get; }
        public bool BlackHasCastle { get; }
        public bool WhiteHasCastle { get; }

        /// <summary>Create a MoveInfo.</summary>
        /// <param name="move">The move played.</param>
        /// <param name="whiteLeftCastle">Whether or not white could left castle.</param>
        /// <param name="whiteRightCastle">Whether or not white could right castle.</param>
        /// <param name="blackLeftCastle">Whether or not black could left castle.</param>
        /// <param name="blackRightCastle">Whether or not black could right castle.</param>
        /// <param name="whiteHasCastle">Whether or not white has castled.</param>
        /// <param name="blackHasCastle">Whether or not black has castled.</param>
        public MoveInfo(Move move, bool whiteLeftCastle, bool whiteRightCastle, bool blackLeftCastle, bool blackRightCastle, bool whiteHasCastle, bool blackHasCastle)
        {
            Move = move;
            WhiteLeftCastle = whiteLeftCastle;
            WhiteRightCastle = whiteRightCastle;
            BlackLeftCastle = blackLeftCastle;
            BlackRightCastle = blackRightCastle;
            WhiteHasCastle = whiteHasCastle;
            BlackHasCastle = blackHasCastle;
        }
    }
}