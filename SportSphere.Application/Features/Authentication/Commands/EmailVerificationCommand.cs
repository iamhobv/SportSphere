using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MagiXSquad.Application.Authentication.DTOs;
using MagiXSquad.Domain.Interfaces.Services;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using static System.Net.WebRequestMethods;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class EmailVerificationCommand : IRequest<ApiResponse<bool>>
    {
        public string Email { get; set; } = null!;
    }

    public class EmailVerificationValidator : AbstractValidator<EmailVerificationCommand>
    {
        public EmailVerificationValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required")
               .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com");
        }

    }
    public class EmailVerificationHandler : IRequestHandler<EmailVerificationCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailService emailService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IValidator<EmailVerificationCommand> validator;

        public EmailVerificationHandler(UserManager<ApplicationUser> userManager, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IValidator<EmailVerificationCommand> validator)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.httpContextAccessor = httpContextAccessor;
            this.validator = validator;
        }

        public async Task<ApiResponse<bool>> Handle(EmailVerificationCommand request, CancellationToken cancellationToken)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);

            }

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "User not found");
            }

            if (user.EmailConfirmed)
            {
                return ApiResponse<bool>.Error(ErrorCode.BadRequest, "Email is already verified");
            }


            var emailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenEncoded = WebUtility.UrlEncode(emailToken);

            var confirmUrl = await emailService.GenerateConfirmUrl(user.Email, tokenEncoded);

            var body = emailService.GenerateConfirmEmailBody(confirmUrl);

            await emailService.SendEmailAsync(request.Email, "Confirm your email", body);

            return ApiResponse<bool>.Ok(true, $"Email verification sent! , {confirmUrl}");
        }




    }

}
