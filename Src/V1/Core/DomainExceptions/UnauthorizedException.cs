namespace MarkItDoneApi.V1.Core.DomainExceptions;

public class UnauthorizedException : Exception
{
    public string Action { get; set; }
    public int StatusCode { get; set; }
    
    public UnauthorizedException(string message) : this(message, null, 401, null)
    {
    }
    
    public UnauthorizedException(string? message = null, string? action = null, int statusCode = 401, Exception? innerException = null) : base(message ?? "Acesso negado.", innerException)
    {
        Action = action ?? "Verifique suas credenciais e tente novamente.";
        StatusCode = statusCode;
    }

    public object ToJson()
    {
        return new
        {
            Name = "UnauthorizedException",
            Message = Message,
            Action = Action,
            StatusCode = StatusCode
        };
    }
}