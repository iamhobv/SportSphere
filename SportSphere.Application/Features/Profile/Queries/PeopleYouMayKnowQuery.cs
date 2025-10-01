using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.Profile.Queries
{
    public class PeopleYouMayKnowQuery : IRequest<ApiResponse<List<FollowerVm>>>
    {
        public string UserId { get; set; } = null!;
    }
    public class PeopleYouMayKnowQueryValidator : AbstractValidator<PeopleYouMayKnowQuery>
    {
        public PeopleYouMayKnowQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }


    public class PeopleYouMayKnowQueryHandler : IRequestHandler<PeopleYouMayKnowQuery, ApiResponse<List<FollowerVm>>>
    {
        private readonly IGenericRepository<SocialProfile> profileRepo;
        private readonly IValidator<PeopleYouMayKnowQuery> _validator;
        private readonly IGenericRepository<Follow> followRepo;
        private readonly IGenericRepository<UserBlock> blockRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public PeopleYouMayKnowQueryHandler(IGenericRepository<SocialProfile> profileRepo, IValidator<PeopleYouMayKnowQuery> validator
            , IGenericRepository<Follow> followRepo, IGenericRepository<UserBlock> blockRepo, UserManager<ApplicationUser> userManager)
        {
            this.profileRepo = profileRepo;
            _validator = validator;
            this.followRepo = followRepo;
            this.blockRepo = blockRepo;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<List<FollowerVm>>> Handle(PeopleYouMayKnowQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<List<FollowerVm>>.Error(ErrorCode.ValidationError, errors);
            }

            var profile = await profileRepo.GetQueryable().AsNoTracking()
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return ApiResponse<List<FollowerVm>>.Error(ErrorCode.NotFound, "User profile not found");


            var visibleFollowingIds = await followRepo.GetQueryable()
            .GroupJoin(
                blockRepo.GetQueryable(),
                f => f.FollowingId,
                b => b.BlockedId,
                (f, blocked) => new { f, blocked }
            )
            .SelectMany(
                x => x.blocked.DefaultIfEmpty(),
                (x, b) => new { x.f, b }
            )
            .Where(x => x.f.FollowerId == profile.UserId && x.b == null)
            .Select(x => x.f.FollowingId)
            .ToListAsync(cancellationToken);

            var suggestedUsers = await profileRepo.GetQueryable().AsNoTracking()
          .Where(u =>
              u.UserId != profile.UserId &&
              !visibleFollowingIds.Contains(u.UserId))
          .Select(u => u.UserId)
          .ToListAsync(cancellationToken);

            if (!suggestedUsers.Any())
                return ApiResponse<List<FollowerVm>>.Ok(new List<FollowerVm>(), "No data");




            var peopleMyILike = await userManager.Users
     .AsNoTracking()
     .Where(u => suggestedUsers.Contains(u.Id))
     .Select(u => new
     {
         u,
         LatestPostDate = u.SocialProfile.Posts
             .OrderByDescending(p => p.CreatedAt)
             .Select(p => p.CreatedAt)
             .FirstOrDefault()
     })
     .OrderByDescending(x => x.LatestPostDate)
     .Take(10)
     .Select(x => new FollowerVm
     {
         ProfileId = x.u.SocialProfile.Id,
         FullName = x.u.FullName,
         Bio = x.u.SocialProfile.Bio,
         ProfilePictureUrl = $"/api/Media/mediaPreview?mediaId={x.u.ProfileImage}",
         UserID = x.u.Id
     })
     .ToListAsync(cancellationToken);


            return ApiResponse<List<FollowerVm>>.Ok(peopleMyILike);
        }
    }

}
