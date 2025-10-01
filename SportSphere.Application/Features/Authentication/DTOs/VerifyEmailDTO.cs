namespace MagiXSquad.Application.Authentication.DTOs
{
    public class VerifyEmailDTO
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
