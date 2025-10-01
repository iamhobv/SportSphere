using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class ResetPasswordCommand : IRequest<ApiResponse<bool>>
    {
        public ResetPasswordDto ResetPasswordData { get; set; }
    }
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.ResetPasswordData.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com")
                .MaximumLength(100);

            RuleFor(x => x.ResetPasswordData.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

            RuleFor(x => x.ResetPasswordData.NewPasswordConfirm)
                .Equal(x => x.ResetPasswordData.NewPassword).WithMessage("Passwords do not match");
        }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<ResetPasswordCommand> validator;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager, IValidator<ResetPasswordCommand> validator)
        {
            _userManager = userManager;
            this.validator = validator;
        }

        public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var ValidateResult = await validator.ValidateAsync(request, cancellationToken);

            if (!ValidateResult.IsValid)
            {
                var errors = string.Join(", ", ValidateResult.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);

            }
            var user = await _userManager.FindByEmailAsync(request.ResetPasswordData.Email);

            if (user == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "User not found.");

            var result = await _userManager.ResetPasswordAsync(user, request.ResetPasswordData.Token, request.ResetPasswordData.NewPassword);

            if (result.Succeeded)
                return ApiResponse<bool>.Ok(true);

            return ApiResponse<bool>.Error(ErrorCode.ValidationError, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

}
