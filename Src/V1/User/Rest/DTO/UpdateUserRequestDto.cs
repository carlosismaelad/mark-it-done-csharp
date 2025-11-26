namespace MarkItDoneApi.V1.User.Rest.DTO;

public record UpdateUserRequestDto(string? Username, string? Email, string? Password);