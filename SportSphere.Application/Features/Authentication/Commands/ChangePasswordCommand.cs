using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class ChangePasswordCommand : IRequest<ApiResponse<bool>>
    {
        public ChangePasswordDto ChangePasswordData { get; set; }
    }
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.ChangePasswordData.CurrentPassword)
                          .NotEmpty().WithMessage("Current Password is required");

            RuleFor(x => x.ChangePasswordData.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

            RuleFor(x => x.ChangePasswordData.NewPasswordConfirm)
                .Equal(x => x.ChangePasswordData.NewPassword).WithMessage("Passwords do not match");
        }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<ChangePasswordCommand> validator;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, IValidator<ChangePasswordCommand> validator, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            this.validator = validator;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {

            var ValidateResult = await validator.ValidateAsync(request, cancellationToken);

            if (!ValidateResult.IsValid)
            {
                var errors = string.Join(", ", ValidateResult.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);

            }

            string? currentUserId = httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You must login first");
            }


            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "User not found.");

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.ChangePasswordData.CurrentPassword,
                request.ChangePasswordData.NewPassword
            );

            if (!result.Succeeded)
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, string.Join(", ", result.Errors.Select(e => e.Description)));

            return ApiResponse<bool>.Ok(true, "Password changed successfully.");
        }
    }
}
