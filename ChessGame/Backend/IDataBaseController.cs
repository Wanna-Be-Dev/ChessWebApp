using System.Collections.Generic;
using System.Data;

namespace ChessGame.DataBase;

public interface IDataBaseController
{
    public bool LogInUser(string username, string password );
    public bool RegisterUser(string username, string password);
}