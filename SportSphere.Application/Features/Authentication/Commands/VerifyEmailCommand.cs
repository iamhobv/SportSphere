using System.ComponentModel.DataAnnotations;
using System.Net;
using MagiXSquad.Application.Authentication.DTOs;
using MagiXSquad.Domain.Interfaces.Services;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class VerifyEmailCommand : IRequest<ApiResponse<EmailVerificationRetDTO>>
    {
        public VerifyEmailDTO VerificationData { get; set; } = null!;
    }
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.VerificationData.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com");

            RuleFor(x => x.VerificationData.Token)
                .NotEmpty().WithMessage("Verification token is required");
        }
    }


    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ApiResponse<EmailVerificationRetDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IValidator<VerifyEmailCommand> validator;

        public VerifyEmailCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService, IValidator<VerifyEmailCommand> validator)
        {
            _userManager = userManager;
            _emailService = emailService;
            this.validator = validator;
        }

        public async Task<ApiResponse<EmailVerificationRetDTO>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var ValidateResult = await validator.ValidateAsync(request, cancellationToken);

            if (!ValidateResult.IsValid)
            {
                var errors = string.Join(", ", ValidateResult.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<EmailVerificationRetDTO>.Error(ErrorCode.ValidationError, errors);

            }
            var user = await _userManager.FindByEmailAsync(request.VerificationData.Email);
            if (user == null)
            {
                return ApiResponse<EmailVerificationRetDTO>.Error(ErrorCode.NotFound, "User not found");
            }

            if (user.EmailConfirmed)
            {
                return ApiResponse<EmailVerificationRetDTO>.Error(ErrorCode.BadRequest, "Email is already verified");
            }


            var decodedToken = WebUtility.UrlDecode(request.VerificationData.Token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<EmailVerificationRetDTO>.Error(ErrorCode.BadRequest, $"Email verification failed: {errors}");
            }

            // Activate the user account
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            // Send welcome email
            // await _emailService.SendWelcomeEmailAsync(user.Email!, user.UserName!);

            var response = new EmailVerificationRetDTO
            {
                Message = "Email verified successfully. Your account is now active.",
                IsVerified = true
            };

            return ApiResponse<EmailVerificationRetDTO>.Ok(response);
        }
    }

}