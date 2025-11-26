namespace MarkItDoneApi.V1.Core.DomainExceptions;

public class NotFoundException(
    
    string? message = null,
    string? action = null,
    int statusCode = 404,
    Exception? innerException = null)
    : Exception(message ?? "Informações não localizadas.", innerException)
{
    private string Action { get; set; } = action ?? "Verifique se os dados buscados estão corretos.";
    public int StatusCode { get; set; } = statusCode;

    public object ToJson()
    {
        return new
        {
            Name = "NotFoundException",
            Message = Message,
            Action = Action,
            StatusCode = StatusCode
        };
    }
}