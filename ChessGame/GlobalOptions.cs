using ChessGame.Models.Player;

namespace ChessGame
{
    public enum GameType
    {
        PlayervsPlayer,
        PlayervsAI,
        AIvsAI
    }
    public static class GlobalOptions
    {
        public static bool IsLoggedIn { get; set; }
        public static string Username { get; set; }
        public static int Score { get; set; }

        public static GameType GameType { get; set; }
    }
}