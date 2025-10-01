using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Profile.Commands
{
    public class DeactivateAccountCommand : IRequest<ApiResponse<bool>>
    {
        public string UserId { get; set; } = null!;
    }
    public class DeactivateAccountCommandValidator : AbstractValidator<DeactivateAccountCommand>
    {
        public DeactivateAccountCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }
    public class DeactivateAccountHandler : IRequestHandler<DeactivateAccountCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeactivateAccountHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<bool>> Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "User not found");

            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != user.Id)
                return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You are not authorized to deactivate this account");

            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;


            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<bool>.Error(ErrorCode.UnknownError, $"Failed to deactivate account: {errors}");
            }


            return ApiResponse<bool>.Ok(true, "Account deactivated successfully");
        }
    }
}
