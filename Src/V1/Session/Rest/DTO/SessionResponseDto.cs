namespace MarkItDoneApi.V1.Session.Rest.DTO;

public record SessionResponseDto(string SessionId, string Message)
{
  public static SessionResponseDto FromEntity(string SessionId, string Message)
  {
    return new SessionResponseDto(SessionId, Message);
  }
};