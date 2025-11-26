namespace MarkItDoneApi.V1.Core.DomainExceptions;

public class NotFoundException : Exception
{
    public string Action { get; set; }
    public int StatusCode { get; set; }
    
    public NotFoundException(string message) : this(message, null, 404, null)
    {
    }
    
    public NotFoundException(string? message = null, string? action = null, int statusCode = 404, Exception? innerException = null) : base(message ?? "Informação não localizada.", innerException)
    {
        Action = action ?? "Ajuste os dados enviados e tente novamente.";
        StatusCode = statusCode;
    }

    public object ToJson()
    {
        return new
        {
            Name = nameof(NotFoundException),
            Message = Message,
            Action = Action,
            StatusCode = StatusCode
        };
    }
}