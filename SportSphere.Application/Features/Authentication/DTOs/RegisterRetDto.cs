using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.DTOs
{
    public class RegisterRetDto
    {
        public string UserId { get; set; } = null!;
        public UserRoles Role { get; set; }
        public string Email { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}