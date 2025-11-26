using System.Security.Cryptography;

namespace MarkItDoneApi.V1.Session.Utils;

public static class SessionUtils
{
    public static string GenerateVerificationCode()
    {
      var randomNum = RandomNumberGenerator.GetInt32(0, 1000000);
      return randomNum.ToString().PadLeft(6, '0');
    }
    
    public static string GenerateToken()
    {
      var bytes = new byte[48];
      RandomNumberGenerator.Fill(bytes);
      return Convert.ToHexString(bytes).ToLower();
    }
}