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
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Users.Commands
{
    public class UnfollowUserCommand : IRequest<ApiResponse<bool>>
    {
        public string UserIdToUnfollow { get; set; } = null!;

    }
    public class UnfollowUserCommandValidator : AbstractValidator<UnfollowUserCommand>
    {
        public UnfollowUserCommandValidator()
        {
            RuleFor(x => x.UserIdToUnfollow)
                .NotEmpty().WithMessage("UserId to unfollow is required");
        }
    }

    public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand, ApiResponse<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<Follow> followRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public UnfollowUserHandler(IHttpContextAccessor httpContextAccessor, IGenericRepository<Follow> followRepo, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            this.followRepo = followRepo;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<bool>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            string? currentUserId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You must login first");
            }
            var UserToUnfollow = await userManager.FindByIdAsync(request.UserIdToUnfollow);


            if (UserToUnfollow == null)
            {
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Please check the users and try again");

            }


            if (currentUserId == request.UserIdToUnfollow)
                return ApiResponse<bool>.Error(ErrorCode.BadRequest, "You cannot unfollow yourself");

            var follow = await followRepo.GetFilteredFirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == request.UserIdToUnfollow);

            if (follow == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "You are not following this user");

            followRepo.ActualDelete(follow);
            await followRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "User unfollowed successfully");
        }
    }


}
