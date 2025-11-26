namespace MarkItDoneApi.V1.Core.DomainExceptions;

public class BusinessException(
    
    string? message = null,
    string? action = null,
    int statusCode = 400,
    Exception? innerException = null)
    : Exception(message ?? "Ocorreu um erro de validação dos dados.", innerException)
{
    private string Action { get; set; } = action ?? "Ajuste os dados enviados e tente novamente.";
    public int StatusCode { get; set; } = statusCode;

    public object ToJson()
    {
        return new
        {
            Name = "BusinessException",
            Message = Message,
            Action = Action,
            StatusCode = StatusCode
        };
    }
}