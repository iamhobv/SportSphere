namespace MagiXSquad.Application.Authentication.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember Me.")]
        public bool RememberMe { get; set; } = false;
    }
}
