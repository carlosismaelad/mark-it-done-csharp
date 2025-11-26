using Microsoft.AspNetCore.Mvc;
using MarkItDoneApi.V1.Email;
using MarkItDoneApi.V1.Email.DTO;
using MarkItDoneApi.V1.Session.Rest.DTO;
using MarkItDoneApi.V1.Session.Service;

namespace MarkItDoneApi.V1.Session.Rest;

[ApiController]
[Route("api/v1/sessions")]
public class SessionController : ControllerBase
{
    private readonly SessionService _sessionService;
    private readonly EmailService _emailService;

    public SessionController(SessionService sessionService, EmailService emailService)
    {
        _sessionService = sessionService;
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePendingSessionAsync([FromBody] SessionRequest request)
    {
        var authenticatedUser = await _sessionService.GetAuthenticatedUserAsync(
            request.Email,
            request.Password
        );

        var newSession = await _sessionService.CreateSessionWithAuthAsync(authenticatedUser.Id);

        await _emailService.SendVerificationEmailAsync(new VerificationEmailData(
            Username: authenticatedUser.Username,
            Code: newSession.Code ?? string.Empty,
            ToEmail: authenticatedUser.Email
        ));

        var response = SessionResponseDto.FromEntity(
            newSession.Id.ToString(),
            "Código de verificação enviado para o seu e-mail!"
        );

        return StatusCode(201, response);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyCodeSentAsync([FromBody] ConfirmSessionDto request)
    {
        var verifiedSession = await _sessionService.VerifySessionCodeAsync(
            Guid.Parse(request.SessionId),
            request.Code
        );

        SetSessionCookie(verifiedSession.Token);

        var response = SessionResponseDto.FromEntity(
            verifiedSession.Id.ToString(),
            "Verificação concluída com sucesso!"
        );

        return Ok(response);
    }

    private void SetSessionCookie(string sessionToken)
    {
        var cookieOptions = new CookieOptions
        {
            Path = "/",
            MaxAge = TimeSpan.FromDays(30), // 30 dias
            Secure = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        };

        Response.Cookies.Append("md_session", sessionToken, cookieOptions);
    }
}