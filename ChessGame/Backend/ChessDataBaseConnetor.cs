using System;
using Npgsql;
namespace ChessGame.DataBase;

public static class ChessDataBaseConnector
{
    public static NpgsqlConnection CreateSession()
    {
        var session = "Host=localhost;Username=postgres;Password=admin;Database=ChessUsers";
        var connection = new NpgsqlConnection(session);
        return connection;
    }
    
}