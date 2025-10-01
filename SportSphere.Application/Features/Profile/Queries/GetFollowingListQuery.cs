using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.Profile.Queries
{
    public class GetFollowingListQuery : IRequest<ApiResponse<List<FollowerVm>>>
    {
        public string UserId { get; set; } = null!;
    }
    public class GetFollowingListQueryValidator : AbstractValidator<GetFollowingListQuery>
    {
        public GetFollowingListQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }
    public class GetFollowingListHandler : IRequestHandler<GetFollowingListQuery, ApiResponse<List<FollowerVm>>>
    {
        private readonly IGenericRepository<SocialProfile> profileRepo;
        private readonly IValidator<GetFollowingListQuery> _validator;
        private readonly IGenericRepository<Follow> followRepo;

        public GetFollowingListHandler(IGenericRepository<SocialProfile> profileRepo, IValidator<GetFollowingListQuery> validator
            , IGenericRepository<Follow> followRepo)
        {
            this.profileRepo = profileRepo;
            _validator = validator;
            this.followRepo = followRepo;
        }

        public async Task<ApiResponse<List<FollowerVm>>> Handle(GetFollowingListQuery request, CancellationToken cancellationToken)
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


            var followings = await followRepo.GetQueryable().AsNoTracking()
               .Where(f => f.FollowerId == profile.UserId).OrderByDescending(a => a.CreatedAt)
               .Take(10)
               .Select(f => new FollowerVm
               {
                   ProfileId = f.Id,
                   FullName = f.Following.FullName,
                   Bio = f.Following.SocialProfile.Bio,
                   ProfilePictureUrl = $"/api/Media/mediaPreview?mediaId={f.Following.ProfileImage}",
                   UserID = f.FollowingId
               })
               .ToListAsync();


            //        var followings = await followRepo.GetQueryable()
            //.AsNoTracking()
            //.Where(f => f.FollowerId == profile.UserId)
            //.Include(f => f.Following).ThenInclude(sp => sp.SocialProfile)
            //.Include(f => f.Follower).ThenInclude(sp => sp.SocialProfile)
            //.Select(f => new
            //{
            //    f,
            //    LatestPostDate = f.Following.SocialProfile.Posts
            //        .OrderByDescending(p => p.CreatedAt)
            //        .Select(p => p.CreatedAt)
            //        .FirstOrDefault()
            //})
            //.OrderByDescending(x => x.LatestPostDate)
            //.Take(10)
            //.Select(x => new FollowerVm
            //{
            //    ProfileId = x.f.Id,
            //    FullName = x.f.Following.FullName,
            //    Bio = x.f.Following.SocialProfile.Bio,
            //    ProfilePictureUrl = $"/api/Media/mediaPreview?mediaId={x.f.Following.ProfileImage}",
            //    UserID = x.f.FollowingId
            //})
            //.ToListAsync();


            return ApiResponse<List<FollowerVm>>.Ok(followings);
        }
    }
}
