namespace MarkItDoneApi.V1.Core.DomainExceptions;

public class ServiceException(
    
    string? message = null,
    string? action = null,
    int statusCode = 503,
    Exception? innerException = null)
    : Exception(message ?? "Serviço indisponível.", innerException)
{
    private string Action { get; set; } = action ?? "Serviço indisponível no momento. Entre em contato com o suporte ou tente mais tarde.";
    public int StatusCode { get; set; } = statusCode;

    public object ToJson()
    {
        return new
        {
            Name = "ServiceException",
            Message = Message,
            Action = Action,
            StatusCode = StatusCode
        };
    }
}
