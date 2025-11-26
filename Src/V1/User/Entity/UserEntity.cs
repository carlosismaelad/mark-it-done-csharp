namespace MarkItDoneApi.V1.User.Entity;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public UserEntity()
    {
    }

    public UserEntity(Guid id, string username, string email, string password, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Username = username;
        Email = email;
        Password = password;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}