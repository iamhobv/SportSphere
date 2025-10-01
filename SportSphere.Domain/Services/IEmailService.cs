namespace MagiXSquad.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string receptor, string subject, string body, bool isBodyHtml = false);
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task<string> GenerateConfirmUrl(string email, string token);

        string GenerateConfirmEmailBody(string confirmUrl);

        Task<string> GenerateResetUrl(string email, string token);

        string GenerateResetEmailBody(string resetUrl, string username);

    }
}