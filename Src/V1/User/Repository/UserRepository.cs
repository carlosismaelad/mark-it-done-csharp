using Dapper;
using MarkItDoneApi.Infra.Data;
using MarkItDoneApi.V1.Core.DomainExceptions;
using MarkItDoneApi.V1.User.Entity;
using MarkItDoneApi.V1.User.Rest.DTO;

namespace MarkItDoneApi.V1.User.Repository;

public class UserRepository
{
    private readonly ConnectionFactory _connectionFactory;
    
    public UserRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task ValidateUniqueUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var count = await connection.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM users WHERE username = @username", 
            new { username });

        if (count > 0)
        {
            throw new BusinessException("Não é possível utilizar esse nome de usuário. Escolha outro nome e tente novamente.");
        }
    }

    public async Task ValidateUniqueEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var count = await connection.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM users WHERE email = @email", 
            new { email });

        if (count > 0)
        {
            throw new BusinessException("Não foi possível utilizar esse e-mail. Escolha outro e-mail e tente novamente.");
        }
    }

    public async Task<UserEntity> CreateAsync(CreateUserRequestDto request)
    {
        using var connection = _connectionFactory.CreateConnection();

        var query = """
                    INSERT INTO users (username, email, password_digest)
                    VALUES (@username, @email, @password)
                    RETURNING *
                    """;

        var newUser = await connection.QuerySingleAsync<UserEntity>(query, new
        {
            username = request.Username,
            email = request.Email,
            password = request.Password
        });

        return newUser;
    }

    public async Task<UserEntity> GetOneByUsername(string username)
    {
        using var connection = _connectionFactory.CreateConnection();

        var selectQuery = """
        SELECT * FROM users WHERE username = @username
        """;

        var userFounded = await connection.QuerySingleOrDefaultAsync<UserEntity>(selectQuery, new
        {
            username
        }) ?? throw new NotFoundException("Usuário não encontrado.");

        return userFounded;
    }

    public async Task<UserEntity> UpdateUser(string username, UpdateUserRequestDto user) 
    {
        using var connection = _connectionFactory.CreateConnection();

        var updateQuery = """
            UPDATE users 
            SET username = @username, 
                email = @email, 
                password_digest = @password, 
                updated_at = NOW()
            WHERE username = @currentUsername
            RETURNING *
            """;

        var updatedUser = await connection.QuerySingleAsync<UserEntity>(updateQuery, new
        {
            username = user.Username,
            email = user.Email,
            password = user.Password,
            currentUsername = username
        });

        return updatedUser;
    }

    public async Task<UserEntity> FindByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        var selectQuery = """
            SELECT 
                id, 
                username, 
                email, 
                password_digest AS Password, 
                created_at AS CreatedAt, 
                updated_at AS UpdatedAt 
            FROM 
                users 
            WHERE 
                email = @email
            """;

        var userFound = await connection.QuerySingleOrDefaultAsync<UserEntity>(selectQuery, new
        {
            email
        }) ?? throw new NotFoundException("Usuário não encontrado.");

        return userFound;
    }
}