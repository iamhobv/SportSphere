using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.DTOs
{
    public class LoginRetDTO
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public UserRoles Role { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}