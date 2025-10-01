using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.Application.Services;
using MagiXSquad.Domain.Interfaces.Services;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class ResendVerificationCommand : IRequest<ApiResponse<bool>>
    {
        public string Email { get; set; } = null!;
    }
    public class ResendVerificationVAlidator : AbstractValidator<ResendVerificationCommand>
    {
        public ResendVerificationVAlidator()
        {
            RuleFor(x => x.Email)
                           .NotEmpty().WithMessage("Email is required")
                           .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com");
        }
    }

    public class ResendVerificationHandler : IRequestHandler<ResendVerificationCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<ResendVerificationCommand> validator;

        public ResendVerificationHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor, IValidator<ResendVerificationCommand> validator)
        {
            _userManager = userManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            this.validator = validator;
        }

        public async Task<ApiResponse<bool>> Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
        {

            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);

            }


            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "User not found");
            }

            if (user.EmailConfirmed)
            {
                return ApiResponse<bool>.Error(ErrorCode.BadRequest, "Email is already verified");
            }

            // Generate new token
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenEncoded = WebUtility.UrlEncode(emailToken);

            var confirmUrl = await _emailService.GenerateConfirmUrl(user.Email, tokenEncoded);
            var body = _emailService.GenerateConfirmEmailBody(confirmUrl);

            await _emailService.SendEmailAsync(request.Email, "Confirm your email", body);

            return ApiResponse<bool>.Ok(true, $"Email verification sent! , {confirmUrl}");
        }
    }

}
