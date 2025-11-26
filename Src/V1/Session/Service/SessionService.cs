using MarkItDoneApi.V1.Core.DomainExceptions;
using MarkItDoneApi.V1.Core.Security;
using MarkItDoneApi.V1.Session.Entity;
using MarkItDoneApi.V1.Session.Repository;
using MarkItDoneApi.V1.User.Entity;
using MarkItDoneApi.V1.User.Repository;

namespace MarkItDoneApi.V1.Session.Service;

public class SessionService
{
    private readonly SessionRepository _sessionRepository;
    private readonly UserRepository _userRepository;

    public SessionService(SessionRepository sessionRepository, UserRepository userRepository)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
    }

    public async Task<SessionEntity> CreateSessionWithAuthAsync(Guid userId)
    {
        var newSession = await _sessionRepository.InsertPendingSessionAsync(userId);
        return newSession;
    }

    public async Task<UserEntity> GetAuthenticatedUserAsync(string providedEmail, string providedPassword)
    {
        try
        {
            var storedUser = await _userRepository.FindByEmailAsync(providedEmail);
            await ValidatePasswordAsync(providedPassword, storedUser.Password);
            return storedUser;
        }
        catch (UnauthorizedException)
        {
            throw new UnauthorizedException(
                message: "Dados de autenticação não conferem.",
                action: "Verifique se os dados enviados estão corretos."
            );
        }
        catch (NotFoundException)
        {
            throw new UnauthorizedException(
                message: "Dados de autenticação não conferem.",
                action: "Verifique se os dados enviados estão corretos."
            );
        }
    }

    public async Task ValidatePasswordAsync(string providedPassword, string storedPassword)
    {
        await Task.Run(() =>
        {
            var correctPasswordMatch = PasswordService.Compare(providedPassword, storedPassword);

            if (!correctPasswordMatch)
            {
                throw new UnauthorizedException(
                    message: "Dados de login não conferem.",
                    action: "Verifique se os dados enviados estão corretos."
                );
            }
        });
    }

    public async Task<SessionEntity> VerifySessionCodeAsync(Guid sessionId, string code)
    {
        var verifiedSession = await _sessionRepository.VerifyCodeAsync(sessionId, code);
        return verifiedSession;
    }
}