namespace MagiXSquad.Application.Authentication.DTOs
{
    public class EmailVerificationRetDTO
    {
        public string Message { get; set; } = null!;
        public bool IsVerified { get; set; }
        public string? LoginToken { get; set; } // Optional: Auto-login after verification
    }
}
