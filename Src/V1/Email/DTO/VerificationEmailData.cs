namespace MarkItDoneApi.V1.Email.DTO;

public record VerificationEmailData(string Username, string Code, string ToEmail);