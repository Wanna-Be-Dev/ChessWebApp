using System.Collections.Generic;
using ChessModel;
using NUnit.Framework;

namespace ChessGameTest
{

    public class Tests
    {
        private ChessBoard _chessBoard;
        
        [SetUp]
        public void Setup()
        {
            _chessBoard = new ChessBoard();
            _chessBoard.Play(new Move(12, 28, _chessBoard.Board[12], _chessBoard.Board[28]));
            _chessBoard.Play(new Move(52, 36, _chessBoard.Board[52], _chessBoard.Board[36]));
            _chessBoard.Play(new Move(6, 21, _chessBoard.Board[6], _chessBoard.Board[21]));
            _chessBoard.Play(new Move(62, 45, _chessBoard.Board[62], _chessBoard.Board[45]));
            _chessBoard.Play(new Move(21, 36, _chessBoard.Board[21], _chessBoard.Board[36]));
            _chessBoard.Play(new Move(45, 28, _chessBoard.Board[45], _chessBoard.Board[28]));
            _chessBoard.Play(new Move(36, 46, _chessBoard.Board[36], _chessBoard.Board[46]));
            _chessBoard.Play(new Move(59, 45, _chessBoard.Board[59], _chessBoard.Board[45]));
            _chessBoard.Play(new Move(3, 12, _chessBoard.Board[3], _chessBoard.Board[12]));
            _chessBoard.Play(new Move(48, 32, _chessBoard.Board[48], _chessBoard.Board[32]));
            _chessBoard.Play(new Move(14, 30, _chessBoard.Board[14], _chessBoard.Board[30]));
            _chessBoard.Play(new Move(32, 24, _chessBoard.Board[32], _chessBoard.Board[24]));
            _chessBoard.Play(new Move(7, 6, _chessBoard.Board[7], _chessBoard.Board[6]));
            _chessBoard.Play(new Move(51, 43, _chessBoard.Board[51], _chessBoard.Board[43]));
            _chessBoard.Play(new Move(9, 25, _chessBoard.Board[9], _chessBoard.Board[25]));
        }

        [Test]
        public void TestCastleFlags()
        {
            Assert.False(_chessBoard.BlackHasCastle);
            Assert.True(_chessBoard.BlackLeftCastle);
            Assert.True(_chessBoard.BlackRightCastle);
            
            Assert.False(_chessBoard.WhiteHasCastle);
            Assert.True(_chessBoard.WhiteLeftCastle);
            Assert.False(_chessBoard.WhiteRightCastle);
        }
        
        [Test]
        public void TestPinnedPiece()
        {
            Assert.True(_chessBoard.GetMoveFromPosition(28).Count == 0);
        }
        
        [Test]
        public void TestKnight()
        {
            _chessBoard.NextToPlay = ChessColor.White;
            List<Move> list = new()
            {
                new Move(1, 16, new Piece(ChessColor.White, 1, ChessType.Knight), new Piece(ChessColor.None, 16, ChessType.None)),
                new Move(1, 18, new Piece(ChessColor.White, 1, ChessType.Knight), new Piece(ChessColor.None, 18, ChessType.None))
            };
            Assert.AreEqual(list, _chessBoard.GetMoveFromPosition(1));
        }
        
        [Test]
        public void TestQueen()
        {
            _chessBoard.NextToPlay = ChessColor.White;
            List<Move> list = new()
            {
                new Move(12, 21, _chessBoard.Board[12], _chessBoard.Board[21]),
                new Move(12, 3, _chessBoard.Board[12], _chessBoard.Board[3]),
                new Move(12, 19, _chessBoard.Board[12], _chessBoard.Board[19]),
                new Move(12, 26, _chessBoard.Board[12], _chessBoard.Board[26]),
                new Move(12, 33, _chessBoard.Board[12], _chessBoard.Board[33]),
                new Move(12, 40, _chessBoard.Board[12], _chessBoard.Board[40]),
                new Move(12, 20, _chessBoard.Board[12], _chessBoard.Board[20]),
                new Move(12, 28, _chessBoard.Board[12], _chessBoard.Board[28])
            };
            Assert.AreEqual(list, _chessBoard.GetMoveFromPosition(12));
        }
        
        [Test]
        public void TestKing()
        {
            _chessBoard.NextToPlay = ChessColor.Black;
            List<Move> list = new()
            {
                new Move(60, 59, _chessBoard.Board[60], _chessBoard.Board[59]),
                new Move(60, 51, _chessBoard.Board[60], _chessBoard.Board[51])
            };
            Assert.AreEqual(list, _chessBoard.GetMoveFromPosition(60));
        }
        
        [Test]
        public void TestEnPassant()
        {
            _chessBoard.NextToPlay = ChessColor.Black;
            Move move = new Move(24,17, _chessBoard.Board[24], _chessBoard.Board[25]); 
            Assert.True(_chessBoard.GetMoveFromPosition(24).Contains(move));
        }
        
        [Test]
        public void TestPawn()
        {
            _chessBoard.NextToPlay = ChessColor.Black;
            List<Move> list = new()
            {
                new Move(55, 47, _chessBoard.Board[55], _chessBoard.Board[47]),
                new Move(55, 39, _chessBoard.Board[55], _chessBoard.Board[39]),
                new Move(55, 46, _chessBoard.Board[55], _chessBoard.Board[46])
            };
            Assert.AreEqual(list, _chessBoard.GetMoveFromPosition(55));
            
            List<Move> list2 = new()
            {
                new Move(43, 35, _chessBoard.Board[43], _chessBoard.Board[35]),
            };
            Assert.AreEqual(list2, _chessBoard.GetMoveFromPosition(43));
        }
        
        [Test]
        public void TestBishop()
        {
            _chessBoard.NextToPlay = ChessColor.Black;
            List<Move> list = new()
            {
                new Move(58, 51, _chessBoard.Board[58], _chessBoard.Board[51]),
                new Move(58, 44, _chessBoard.Board[58], _chessBoard.Board[44]),
                new Move(58, 37, _chessBoard.Board[58], _chessBoard.Board[37]),
                new Move(58, 30, _chessBoard.Board[58], _chessBoard.Board[30])
            };
            Assert.AreEqual(list, _chessBoard.GetMoveFromPosition(58));
        }
        
        [Test]
        public void TestCastle()
        {
            _chessBoard.NextToPlay = ChessColor.White;
            _chessBoard.Play(new Move(1, 18, _chessBoard.Board[1], _chessBoard.Board[18]));
            _chessBoard.NextToPlay = ChessColor.White;
            _chessBoard.Play(new Move(2, 9, _chessBoard.Board[2], _chessBoard.Board[9]));
            _chessBoard.NextToPlay = ChessColor.White;

            List<Move> list = new()
            {
                new Move(4, 3, _chessBoard.Board[4], _chessBoard.Board[3]),
                new Move(4, 2, _chessBoard.Board[4], _chessBoard.Board[2])
            };
            Assert.AreEqual(list, _chessBoard.GetMoveFromPosition(4));        
            
            _chessBoard.Play( new Move(4, 2, _chessBoard.Board[4], _chessBoard.Board[2]));
            
            Assert.True(_chessBoard.Board[3].Type == ChessType.Rook);
            Assert.True(_chessBoard.Board[0].Type == ChessType.None);
            Assert.True(_chessBoard.Board[2].Type == ChessType.King);
            Assert.True(_chessBoard.Board[4].Type == ChessType.None);
            
            Assert.True(_chessBoard.WhiteHasCastle);
            Assert.False(_chessBoard.WhiteLeftCastle);
            Assert.False(_chessBoard.WhiteRightCastle);
        }
    }
}