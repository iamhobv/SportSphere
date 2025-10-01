using SportSphere.Domain.Enums;

namespace MagiXSquad.Application.Authentication.DTOs
{
    public class RegisterDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; } = null!;
        public UserRoles Role { get; set; }
        public GenderType Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }


        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com")
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(\+20|0)?1[0-9]{9}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number must be a valid mobile number format, e.g. +201234567890 or 01234567890");
        }
    }
}