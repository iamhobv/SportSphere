

using MagiXSquad.Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using SportSphere.Domain.Services;

namespace MagiXSquad.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SmtpOptions _options;
        public EmailService(IConfiguration configuration, IOptions<SmtpOptions> options, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        public async Task<bool> SendEmail(string receptor, string subject, string body, bool isBodyHtml = false)
        {
            var port = _configuration.GetValue<int>("EmailSettingsConfiguration:Port");
            var host = _configuration.GetValue<string>("EmailSettingsConfiguration:Host");
            var email = _configuration.GetValue<string>("EmailSettingsConfiguration:Email");
            var password = _configuration.GetValue<string>("EmailSettingsConfiguration:Password");

            var smtpClient = new System.Net.Mail.SmtpClient(host, port);

            smtpClient.EnableSsl = true;

            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new System.Net.NetworkCredential(email, password);

            var message = new System.Net.Mail.MailMessage(email!, receptor, subject, body);

            message.IsBodyHtml = isBodyHtml;

            await smtpClient.SendMailAsync(message);

            return true;
        }


        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_options.User, _options.Pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task<string> GenerateConfirmUrl(string email, string token)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new InvalidOperationException("No active HTTP context found.");

            // https://localhost:7212/api/Authentication/verify-email?Email=test2@gmail.com&Token=898230
            return $"{request.Scheme}://{request.Host}/api/Authentication/verify-email?Email={email}&Token={token}";
        }

        public string GenerateConfirmEmailBody(string confirmUrl)
        {
            var body = $@"<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <title>Email Confirmation</title>
</head>
<body style='margin:0; padding:0; background-color:#f5f5f5; font-family: Arial, sans-serif;'>
  <table align='center' width='100%' cellpadding='0' cellspacing='0' style='background-color:#f5f5f5; padding:40px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff; border-radius:8px; box-shadow:0 2px 6px rgba(0,0,0,0.1); padding:40px; text-align:center;'>
          <tr>
            <td style='text-align:center;'>
              <h2 style='color:#333; margin-bottom:20px;'>Email Confirmation</h2>
              <p style='color:#888; font-size:14px; margin-bottom:25px;'>
                Thanks for signing up to SportSphere,We’re happy to have you.
              </p>

              <p style='color:#666; font-size:16px; margin-bottom:30px;'>
                Please confirm your email address by clicking the button below:
              </p>
              <a href='{confirmUrl}' 
                 style='display:inline-block; padding:12px 24px; font-size:16px; color:#fff; background:#e91e63; 
                        text-decoration:none; border-radius:25px; font-weight:bold;'>
                Confirm My Email Address
              </a>
              <p style='color:#999; font-size:14px; margin-top:30px;'>
                If you did not create this account, you can safely ignore this email.
              </p>
            </td>
          </tr>
        </table>
        <p style='color:#999; font-size:12px; margin-top:20px;'>
          © {DateTime.Now.Year} SportSphere. All rights reserved.
        </p>
      </td>
    </tr>
  </table>
</body>
</html>";

            return body;
        }


        public async Task<string> GenerateResetUrl(string email, string token)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new InvalidOperationException("No active HTTP context found.");

            // https://localhost:7212/api/Authentication/verify-email?Email=test2@gmail.com&Token=898230
            //reset-password?email={user.Email}&token={encodedToken}
            return $"http://localhost:4200/auth/reset-password?Email={email}&Token={token}";
        }

        public string GenerateResetEmailBody(string resetUrl, string username)
        {
            var body = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <title>Password Reset</title>
</head>
<body style='margin:0; padding:0; background-color:#f5f5f5; font-family: Arial, sans-serif;'>
  <table align='center' width='100%' cellpadding='0' cellspacing='0' style='background-color:#f5f5f5; padding:40px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff; border-radius:8px; box-shadow:0 2px 6px rgba(0,0,0,0.1); padding:40px; text-align:center;'>
          <tr>
            <td>
              <h2 style='color:#333;'>Password Reset Request</h2>
              <p style='color:#666; font-size:16px;'>
                Hi {username}, <br/>
                You requested to reset your password. Click the button below to set a new one:
              </p>
              <a href='{resetUrl}' 
                 style='display:inline-block; margin-top:20px; padding:12px 24px; font-size:16px; 
                        color:#fff; background:#007bff; text-decoration:none; border-radius:25px; font-weight:bold;'>
                Reset My Password
              </a>
              <p style='color:#999; font-size:14px; margin-top:30px;'>
                If you didn’t request this, just ignore this email. Your account is safe.
              </p>
            </td>
          </tr>
        </table>
        <p style='color:#999; font-size:12px; margin-top:20px;'>
          © {DateTime.Now.Year} YourCompany. All rights reserved.
        </p>
      </td>
    </tr>
  </table>
</body>
</html>";

            return body;
        }

    }
}