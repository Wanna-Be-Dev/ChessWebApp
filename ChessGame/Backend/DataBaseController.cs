using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
namespace ChessGame.DataBase;

public class DataBaseController: IDataBaseController
{
    private readonly NpgsqlConnection _dbConnection;

    public DataBaseController()
    {
        this._dbConnection = ChessDataBaseConnector.CreateSession();
    }
    
    public object Score { get; set; }

    public bool LogInUser(string username, string password)
    {
        var hashedPassword = Hasher.HashString(password);
        var sql = 
            @$"
            SELECT * from ""Accounts"" Ac
            WHERE Ac.username like '{username}' 
              and Ac.hash_password like '{hashedPassword}'
            ";
        _dbConnection.Open();
        using var cmd = new NpgsqlCommand(sql, _dbConnection);
        var result = cmd.ExecuteScalar()?.ToString();
        _dbConnection.Close();
        return result != null;
    }

    public bool RegisterUser(string username, string password)
    {
        var hashedPassword = Hasher.HashString(password);


        var sql = 
            @$"
            INSERT INTO ""Accounts""
            (username, hash_password)
            select '{username}', '{hashedPassword}'
            WHERE
                NOT EXISTS (
                    SELECT * FROM ""Accounts""
                            WHERE username = '{username}')
            ";
        _dbConnection.Open();
        using var cmd = new NpgsqlCommand(sql, _dbConnection);
        var result = cmd.ExecuteNonQuery();
        _dbConnection.Close();
        return result > 0 ? true : false;
    }
}