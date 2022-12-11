using System.Collections.Generic;
using System.Linq;
using ChessGame.Models.Player;
using ChessModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ChessGame.Pages
{
    public class ChessModel : PageModel
    {
        private readonly ILogger<ChessModel> _logger;
        private readonly IChessBoard _board;
        private readonly MinmaxPlayer _player;

        public ChessModel(ILogger<ChessModel> logger, IChessBoard chessBoard)
        {
            _logger = logger;
            _board = chessBoard;
            _player = new MinmaxPlayer(2, _board);
        }
        
        public void OnGet()
        {
            _board.InitializeBoard();
        }

        public ActionResult OnPost(string data)
        {
            Response.StatusCode = 200;
            if (data == null) return new EmptyResult();
            JObject? result;
            
            string[] info = data.Split(" ");
            result = info.Length == 1 ? PromotePawn(info) : PlayMove(info);
            

            if (result == null) return new EmptyResult();
            result["end"] = TestEndOfGame();
            return Content(result.ToString());
        }

        private JObject PlayIa()
        {
            JObject json = new() {["whitePromote"] = "false", ["blackPromote"] = "false"};
            var moves = new List<Move> {_player.GetDesiredMove(_board.NextToPlay)};
            _board.SetMoves(moves);
            _board.Play(moves[0]);
            if (_board.PromoteWhite >= 0)
            {
                json["whitePromote"] = "true";
                _board.PromotePawn(ChessType.Queen, ChessColor.White);
            }

            if (_board.PromoteBlack >= 0)
            {
                json["blackPromote"] = "true";
                _board.PromotePawn(ChessType.Queen, ChessColor.Black);
            }

            json["moves"] = JToken.FromObject(_board.GetMoves());
            _board.GetMoves().Clear();
            return json;
        }

        private JObject? PlayMove(string[] info)
        {
            if (GlobalOptions.GameType is GameType.PlayervsPlayer or GameType.PlayervsAI)
            {
                JObject json = new() {["select"] = "false", ["whitePromote"] = "false", ["blackPromote"] = "false"};

                var position = int.Parse(info[0]);
                _board.SetMoves(_board.GetMoves().Where(move => move.EndPosition.Equals(position)).ToList());
                if (_board.GetMoves().Count == 1)
                {
                    var move = _board.GetMoves()[0];
                    _board.Play(move);
                    _logger.LogCritical(move.ToString());
                    if (_board.PromoteWhite >= 0)
                        json["whitePromote"] = "true";
                    var playerMoves = new List<Move>(_board.GetMoves());
                    json["playerMoves"] = JToken.FromObject(playerMoves);
                    _board.GetMoves().Clear();
                    if (GlobalOptions.GameType is GameType.PlayervsAI)
                    {
                        var aiMove = new List<Move> {_player.GetDesiredMove()};
                        _board.SetMoves(aiMove);
                        _board.Play(aiMove[0]);
                        if (_board.PromoteBlack >= 0)
                        {
                            json["blackPromote"] = "true";
                            _board.PromotePawn(ChessType.Queen, ChessColor.Black);
                        }
                    }

                    json["AImoves"] = JToken.FromObject(_board.GetMoves());
                    _board.GetMoves().Clear();
                    return json;
                }

                _board.SetMoves(_board.GetMoveFromPosition(position));
                json["playerMoves"] = JToken.FromObject(_board.GetMoves());
                json["select"] = "true";

                return _board.GetMoves().Any() ? json : null;
            }

            return null;
        }

        private JObject PromotePawn(string[] info)
        {
            JObject json = new();
            switch (info[0])
            {
                case "Queen":
                    _board.PromotePawn(ChessType.Queen, ChessColor.White);
                    json["type"] = "Queen";
                    break;
                case "Rook":
                    _board.PromotePawn(ChessType.Rook, ChessColor.White);
                    json["type"] = "Rook";
                    break;
                case "Bishop":
                    _board.PromotePawn(ChessType.Bishop, ChessColor.White);
                    json["type"] = "Bishop";
                    break;
                case "Knight":
                    _board.PromotePawn(ChessType.Knight, ChessColor.White);
                    json["type"] = "Knight";
                    break;
            }

            return json;
        }

        private string TestEndOfGame()
        {
            return _board.IsCheckMate ? _board.NextToPlay == ChessColor.White ? "black" : "white" : _board.IsDraw() ? "draw" : "no";
        }
    }
}