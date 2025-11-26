namespace MarkItDoneApi.V1.Core.Security;

public static class PasswordService
{
    public static string Hash(string password)
    {
        var rounds = GetNumberOfRounds();
        return BCrypt.Net.BCrypt.HashPassword(password, rounds);
    }
    
    public static bool Compare(string providedPassword, string storedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, storedPassword);
    }

    private static int GetNumberOfRounds()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return environment == "Production" ? 14 : 4;
    }
}