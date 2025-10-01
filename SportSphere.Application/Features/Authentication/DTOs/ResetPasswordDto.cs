using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Application.Features.Authentication.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
    public class ResetPasswordDtoVAlidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoVAlidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com")
                .MaximumLength(100);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

            RuleFor(x => x.NewPasswordConfirm)
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match");

        }
    }

}
