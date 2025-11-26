using MailKit.Net.Smtp;
using MimeKit;
using MarkItDoneApi.V1.Email.DTO;

namespace MarkItDoneApi.V1.Email;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SmtpClient CreateTransporter()
    {
        var emailProvider = _configuration["EMAIL_PROVIDER"] ?? "mailcatcher";

        Console.WriteLine("🔧 Email Debug:");
        Console.WriteLine($"EMAIL_PROVIDER: {emailProvider}");
        Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development"}");

        var smtpClient = new SmtpClient();

        if (emailProvider == "gmail")
        {
            var gmailUsername = _configuration["GMAIL_USERNAME"];
            var gmailPassword = _configuration["GMAIL_APP_PASSWORD"];

            if (string.IsNullOrEmpty(gmailUsername) || string.IsNullOrEmpty(gmailPassword))
            {
                throw new InvalidOperationException(
                    "Gmail credentials not configured. Check GMAIL_USERNAME and GMAIL_APP_PASSWORD environment variables."
                );
            }

            Console.WriteLine("📧 Using Gmail SMTP");
            smtpClient.Connect("smtp.gmail.com", 587, false);
            smtpClient.Authenticate(gmailUsername, gmailPassword);
        }
        else
        {
            Console.WriteLine("📧 Using MailCatcher SMTP");
            var mailcatcherHost = _configuration["MAILCATCHER_HOST"] ?? "localhost";
            var mailcatcherPort = int.Parse(_configuration["MAILCATCHER_PORT"] ?? "1025");

            smtpClient.Connect(mailcatcherHost, mailcatcherPort, false);
            // MailCatcher não precisa de autenticação
        }

        return smtpClient;
    }

    private static string GetVerificationEmailTemplate(string username, string code)
    {
        return $@"
<!DOCTYPE html>
<html>
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>Código de Verificação</title>
    <style>
      body {{
        font-family: Arial, sans-serif;
        line-height: 1.6;
        margin: 0;
        padding: 20px;
        background-color: #f4f4f4;
      }}
      .container {{
        max-width: 600px;
        margin: 0 auto;
        background-color: white;
        padding: 30px;
        border-radius: 8px;
        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
      }}
      .header {{
        text-align: center;
        margin-bottom: 30px;
      }}
      .logo {{
        font-size: 24px;
        font-weight: bold;
        color: #c2410c;
        margin-bottom: 10px;
      }}
      .code-container {{
        text-align: center;
        margin: 30px 0;
        padding: 20px;
        background-color: #f8fafc;
        border-radius: 6px;
        border: 2px dashed #e2e8f0;
      }}
      .code {{
        font-size: 32px;
        font-weight: bold;
        color: #1e293b;
        letter-spacing: 4px;
        font-family: 'Courier New', monospace;
      }}
      .warning {{
        background-color: #fef3c7;
        border: 1px solid #f59e0b;
        border-radius: 6px;
        padding: 15px;
        margin: 20px 0;
        color: #92400e;
      }}
      .footer {{
        text-align: center;
        margin-top: 30px;
        color: #64748b;
        font-size: 14px;
      }}
    </style>
  </head>
  <body>
    <div class=""container"">
      <div class=""header"">
        <div class=""logo"">MarkIt Done</div>
        <h1>Código de Verificação</h1>
      </div>

      <p>Olá, <strong>{username}</strong>!</p>

      <p>Recebemos uma tentativa de login em sua conta. Para concluir o acesso, use o código de verificação abaixo:</p>

      <div class=""code-container"">
        <div class=""code"">{code}</div>
      </div>

      <div class=""warning"">
        <strong>⚠️ Importante:</strong><br>
        • Este código expira em <strong>5 minutos</strong><br>
        • Não compartilhe este código com ninguém<br>
        • Se você não tentou fazer login, ignore este email
      </div>

      <p>Se você não conseguir usar o código, você pode solicitar um novo código na tela de login.</p>

      <div class=""footer"">
        <p>Este é um email automático. Por favor, não responda.</p>
        <p>MarkIt Done - Seu Sistema de Gerenciamento De Compras</p>
      </div>
    </div>
  </body>
</html>";
    }

    public async Task SendVerificationEmailAsync(VerificationEmailData emailData)
    {
        var message = new MimeMessage();
        
        var emailProvider = _configuration["EMAIL_PROVIDER"] ?? "mailcatcher";
        var fromAddress = emailProvider == "gmail" ? _configuration["GMAIL_USERNAME"] : "noreply@markitdone.local";
        
        message.From.Add(new MailboxAddress("MarkIt Done", fromAddress ?? "noreply@markitdone.com"));
        message.To.Add(new MailboxAddress("", emailData.ToEmail));
        message.Subject = "Código de Verificação - MarkIt Done";

        var htmlContent = GetVerificationEmailTemplate(emailData.Username, emailData.Code);
        var textContent = $"Olá {emailData.Username}! Seu código de verificação é: {emailData.Code}. Este código expira em 5 minutos.";

        var builder = new BodyBuilder
        {
            HtmlBody = htmlContent,
            TextBody = textContent
        };
        
        message.Body = builder.ToMessageBody();

        using var smtp = CreateTransporter();
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var smtp = CreateTransporter();
            await smtp.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na conexão SMTP: {ex.Message}");
            return false;
        }
    }
}