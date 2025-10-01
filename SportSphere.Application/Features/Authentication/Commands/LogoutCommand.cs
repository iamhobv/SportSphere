using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using SportSphere.Domain.Entities;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class LogoutCommand : IRequest<ApiResponse<bool>>
    {
        public string UserId { get; set; } = null!;
    }

    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
        }
    }
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<bool>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutCommandHandler(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();

            return ApiResponse<bool>.Ok(true, "You have been logged out successfully.");
        }
    }
}
