namespace MarkItDoneApi.V1.Core.DomainExceptions;

public class UnauthorizedException(
    
    string? message = null,
    string? action = null,
    int statusCode = 401,
    Exception? innerException = null)
    : Exception(message ?? "Acesso negado.", innerException)
{
    private string Action { get; set; } = action ?? "Verifique suas credenciais e tente novamente.";
    public int StatusCode { get; set; } = statusCode;

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