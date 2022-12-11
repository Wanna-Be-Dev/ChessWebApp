using System;

namespace ChessGame.DataBase;

public static class Hasher
{
    public static string HashString(string text, string salt = "Kursovaya_Rabota")
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }
    
        // Uses SHA256 to create the hash
#pragma warning disable SYSLIB0021
        using var sha = new System.Security.Cryptography.SHA256Managed();
#pragma warning restore SYSLIB0021
        // Convert the string to a byte array first, to be processed
        var textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
        var hashBytes = sha.ComputeHash(textBytes);
        
        // Convert back to a string, removing the '-' that BitConverter adds
        var hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", String.Empty);

        return hash;
    }
}