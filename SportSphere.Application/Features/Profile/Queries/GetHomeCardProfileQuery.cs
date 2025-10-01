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
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Profile.Queries
{
    public class GetHomeCardProfileQuery : IRequest<ApiResponse<HomeProfileCard>>
    {
        public string UserId { get; set; } = null!;

    }
    public class GetHomeCardProfileQueryVAlidator : AbstractValidator<GetHomeCardProfileQuery>
    {
        public GetHomeCardProfileQueryVAlidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");

        }
    }
    public class GetHomeCardProfileQueryHandler : IRequestHandler<GetHomeCardProfileQuery, ApiResponse<HomeProfileCard>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GetHomeCardProfileQueryHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<HomeProfileCard>> Handle(GetHomeCardProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                string? currentUserId = httpContextAccessor.HttpContext?.User?
                                        .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return ApiResponse<HomeProfileCard>.Error(ErrorCode.Unauthorized, "You must login first");
                }


                bool isCurrentUser = request.UserId == currentUserId;



                var userData = context.Users.AsNoTracking().Where(u => u.Id == request.UserId).
                    Select(userData => new HomeProfileCard
                    {
                        UserId = userData.Id,
                        FullName = userData.FullName,
                        ProfileImage = userData.ProfileImage,
                        Bio = userData.SocialProfile.Bio,
                        Country = userData.SocialProfile.Country,
                        City = userData.SocialProfile.City,
                        Role = userData.Role,
                        Sport = (userData.SocialProfile as AthleteProfile).Sport,
                        ProfileID = userData.SocialProfile.Id,
                        IsIFollow = userData.SocialProfile.User.Followers.Any(a => a.FollowerId.Equals(currentUserId)),
                        IsIBlock = userData.SocialProfile.User.Blocked.Any(b => b.BlockerId.Equals(currentUserId)),
                        ProfileImageURL = $"/api/Media/mediaPreview?mediaId={userData.ProfileImage}"

                    })
                    .FirstOrDefault(u => u.UserId == request.UserId);

                if (userData == null)
                    return ApiResponse<HomeProfileCard>.Error(ErrorCode.NotFound, "User not found");





                return ApiResponse<HomeProfileCard>.Ok(userData);
            }
            catch (Exception)
            {
                return ApiResponse<HomeProfileCard>.Error(ErrorCode.UnknownError, "UnExpected error");


            }
        }
    }
}
