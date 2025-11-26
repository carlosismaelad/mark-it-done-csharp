using System.Text.RegularExpressions;
using Dapper;
using MarkItDoneApi.Infra.Data;
using MarkItDoneApi.V1.Core.DomainExceptions;
using MarkItDoneApi.V1.Session.Entity;
using MarkItDoneApi.V1.Session.Utils;

namespace MarkItDoneApi.V1.Session.Repository;

public class SessionRepository
{
    private readonly ConnectionFactory _connectionFactory;
    private static readonly double ExpirationSessionMilliseconds = TimeSpan.FromDays(30).TotalMilliseconds;
    private static readonly double ExpirationCodeMilliseconds = TimeSpan.FromMinutes(5).TotalMilliseconds;

    public SessionRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SessionEntity> InsertPendingSessionAsync(Guid userId)
    {
        var token = SessionUtils.GenerateToken();
        var expiresAt = DateTime.UtcNow.AddMilliseconds(ExpirationSessionMilliseconds);
        var code = SessionUtils.GenerateVerificationCode();
        var codeExpiresAt = DateTime.UtcNow.AddMilliseconds(ExpirationCodeMilliseconds);

        const string sql = @"
            INSERT INTO sessions (user_id, token, expires_at, code, code_expires_at, status, code_attempts)
            VALUES (@userId, @token, @expiresAt, @code, @codeExpiresAt, 'pending_verification', 0)
            RETURNING id, user_id, token, expires_at, code, code_expires_at, code_attempts, status, created_at, updated_at;
        ";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SessionEntity>(sql, new
        {
            userId,
            token,
            expiresAt,
            code,
            codeExpiresAt
        });
    }

    public async Task<SessionEntity> VerifyCodeAsync(Guid sessionId, string inputCode)
    {
        var session = await FindSessionByIdAsync(sessionId);

        if (session.Status == "expired")
        {
            throw new UnauthorizedException(
                message: "Sessão expirada.",
                action: "Solicite um novo código de verificação."
            );
        }

        var isValidCode = session.Code?.ToString() == inputCode && 
                         session.CodeExpiresAt.HasValue && 
                         session.CodeExpiresAt.Value > DateTime.UtcNow;

        if (!isValidCode)
        {
            var newAttempts = await IncreaseVerificationAttemptsAsync(sessionId);

            if (newAttempts >= 3)
            {
                await ExpireSessionAsync(sessionId);
                throw new UnauthorizedException(
                    message: "Máximo de tentativas atingido. Inicie uma nova sessão.",
                    action: "Solicite um novo código de verificação."
                );
            }

            throw new UnauthorizedException(
                message: "Código inválido!",
                action: "Verifique o código no seu e-mail e tente novamente."
            );
        }

        await ActivateSessionAsync(sessionId);
        return session;
    }

    public async Task<SessionEntity> FindSessionByIdAsync(Guid sessionId)
    {
        const string sql = @"
            SELECT 
                id,
                user_id AS UserId,
                token,
                expires_at AS ExpiresAt,
                code,
                code_expires_at AS CodeExpiresAt,
                code_attempts AS CodeAttempts,
                status,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM 
                sessions 
            WHERE 
                id = @sessionId
            ";

        using var connection = _connectionFactory.CreateConnection();
        var session = await connection.QuerySingleOrDefaultAsync<SessionEntity>(sql, new { sessionId });

        if (session == null)
        {
            throw new NotFoundException(
                message: "Sessão não encontrada.",
                action: "Verifique se o ID da sessão está correto ou tente criar uma nova sessão."
            );
        }

        return session;
    }

    public async Task ActivateSessionAsync(Guid sessionId)
    {
        const string sql = @"
            UPDATE sessions 
            SET status = 'active', updated_at = timezone('utc', now())
            WHERE id = @sessionId
        ";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { sessionId });
    }

    public async Task<int> IncreaseVerificationAttemptsAsync(Guid sessionId)
    {
        const string sql = @"
            UPDATE sessions 
            SET code_attempts = code_attempts + 1, updated_at = timezone('utc', now())
            WHERE id = @sessionId
            RETURNING code_attempts
        ";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, new { sessionId });
    }

    public async Task ExpireSessionAsync(Guid sessionId)
    {
        const string sql = @"
            UPDATE sessions 
            SET status = 'expired', updated_at = timezone('utc', now())
            WHERE id = @sessionId
        ";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { sessionId });
    }
}