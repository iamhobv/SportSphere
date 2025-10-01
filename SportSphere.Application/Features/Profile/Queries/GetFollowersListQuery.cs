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
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Profile.Queries
{
    public class GetFollowersListQuery : IRequest<ApiResponse<List<FollowerVm>>>
    {
        public string UserId { get; set; } = null!;
    }
    public class GetFollowersListQueryValidator : AbstractValidator<GetFollowersListQuery>
    {
        public GetFollowersListQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }
    public class GetFollowersListHandler : IRequestHandler<GetFollowersListQuery, ApiResponse<List<FollowerVm>>>
    {
        private readonly IGenericRepository<SocialProfile> profileRepo;
        private readonly IValidator<GetFollowersListQuery> _validator;
        private readonly IGenericRepository<Follow> followRepo;

        public GetFollowersListHandler(IGenericRepository<SocialProfile> profileRepo, IValidator<GetFollowersListQuery> validator
            , IGenericRepository<Follow> followRepo)
        {
            this.profileRepo = profileRepo;
            _validator = validator;
            this.followRepo = followRepo;
        }

        public async Task<ApiResponse<List<FollowerVm>>> Handle(GetFollowersListQuery request, CancellationToken cancellationToken)
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



            var followers = await followRepo.GetQueryable().AsNoTracking()
                .Where(f => f.FollowingId == profile.UserId)
                .Include(f => f.Follower).ThenInclude(a => a.SocialProfile)
                .Select(f => new FollowerVm
                {
                    ProfileId = f.Id,
                    FullName = f.Follower.FullName,
                    Bio = f.Follower.SocialProfile.Bio,
                    ProfilePictureUrl = $"/api/Media/mediaPreview?mediaId={f.Follower.ProfileImage}"
                })
                .ToListAsync();

            return ApiResponse<List<FollowerVm>>.Ok(followers);
        }
    }


}
