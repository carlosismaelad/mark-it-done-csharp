namespace MarkItDoneApi.V1.Core.DomainExceptions;

public class BusinessException : Exception
{
    public string Action { get; set; }
    public int StatusCode { get; set; }
    
    public BusinessException(string message) : this(message, null, 400, null)
    {
    }
    
    public BusinessException(string? message = null, string? action = null, int statusCode = 400, Exception? innerException = null) : base(message ?? "Um erro de validação ocorreu.", innerException)
    {
        Action = action ?? "Ajuste os dados enviados e tente novamente.";
        StatusCode = statusCode;
    }

    public object ToJson()
    {
        return new
        {
            Name = nameof(BusinessException),
            Message = Message,
            Action = Action,
            StatusCode = StatusCode
        };
    }
}