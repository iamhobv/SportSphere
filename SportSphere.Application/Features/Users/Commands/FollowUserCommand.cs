using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Users.Commands
{
    public class FollowUserCommand : IRequest<ApiResponse<bool>>
    {
        public string UserIdToFollow { get; set; } = null!;
    }
    public class FollowUserCommandValidator : AbstractValidator<FollowUserCommand>
    {
        public FollowUserCommandValidator()
        {
            RuleFor(x => x.UserIdToFollow)
                .NotEmpty().WithMessage("UserId to follow is required");
        }
    }
    public class FollowUserHandler : IRequestHandler<FollowUserCommand, ApiResponse<bool>>
    {
        private readonly IGenericRepository<Follow> followRepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FollowUserHandler(IGenericRepository<Follow> followRepo, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.followRepo = followRepo;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<bool>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {

            string? currentUserId = httpContextAccessor.HttpContext?.User?
.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You must login first");
            }


            var UserToFollow = await userManager.FindByIdAsync(request.UserIdToFollow);
            if (UserToFollow == null)
            {
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Please check the user and try again");

            }



            if (currentUserId == request.UserIdToFollow)
                return ApiResponse<bool>.Error(ErrorCode.BadRequest, "You cannot follow yourself");


            var alreadyFollowing = await followRepo
                .GetFilteredFirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowingId == request.UserIdToFollow);

            if (alreadyFollowing != null)
                return ApiResponse<bool>.Error(ErrorCode.Conflict, "You are already following this user");

            var follow = new Follow
            {
                FollowerId = currentUserId,
                FollowingId = request.UserIdToFollow,
                CreatedAt = DateTime.UtcNow
            };

            await followRepo.AddAsync(follow);
            await followRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "User followed successfully");
        }
    }

}
