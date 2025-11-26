namespace MarkItDoneApi.V1.Session.Entity;

public class SessionEntity
{
    public Guid Id { get; set; }
  
    public Guid UserId { get; set; }
  
    public string Token { get; set; } = string.Empty;  
    public DateTime ExpiresAt { get; set; }  
    public string? Code { get; set; }
    public DateTime? CodeExpiresAt { get; set; }
    public int CodeAttempts { get; set; } = 0;
    public string Status { get; set; } = "pending_verification";  
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  
}