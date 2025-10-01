using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.Domain.Interfaces.Services;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class ForgotPasswordCommand : IRequest<ApiResponse<bool>>
    {
        public ForgotPasswordDto ForgotPasswordData { get; set; }
    }
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.ForgotPasswordData.Email)
                           .NotEmpty().WithMessage("Email is required")
                           .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com");
        }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService emailService;
        private readonly IValidator<ForgotPasswordCommand> validator;

        public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration config, IEmailService emailService, IValidator<ForgotPasswordCommand> validator)
        {
            _userManager = userManager;
            _config = config;
            this.emailService = emailService;
            this.validator = validator;
        }

        public async Task<ApiResponse<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {

            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);

            }
            var email = request.ForgotPasswordData.Email;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Email address not found or not confirmed.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEncoded = WebUtility.UrlEncode(token);


            var resetUrl = await emailService.GenerateResetUrl(user.Email, tokenEncoded);

            // Email template
            var body = emailService.GenerateResetEmailBody(resetUrl, user.UserName);

            await emailService.SendEmailAsync(email, "Reset your password", body);

            return ApiResponse<bool>.Ok(true, $"reset password sent !");
        }


    }

}
