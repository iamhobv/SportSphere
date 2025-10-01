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
    public class NewProfileStats : IRequest<ApiResponse<UserStats>>
    {
        public string UserId { get; set; } = null!;
    }
    public class NewProfileStatsValidator : AbstractValidator<NewProfileStats>
    {
        public NewProfileStatsValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }
    public class NewProfileStatsHandler : IRequestHandler<NewProfileStats, ApiResponse<UserStats>>
    {
        private readonly IGenericRepository<SocialProfile> profileRepo;
        private readonly IValidator<NewProfileStats> _validator;
        private readonly IGenericRepository<Follow> followRepo;
        private readonly IGenericRepository<Post> postRepo;

        public NewProfileStatsHandler(IGenericRepository<SocialProfile> profileRepo, IValidator<NewProfileStats> validator
            , IGenericRepository<Follow> followRepo, IGenericRepository<Post> postRepo)
        {
            this.profileRepo = profileRepo;
            _validator = validator;
            this.followRepo = followRepo;
            this.postRepo = postRepo;
        }

        public async Task<ApiResponse<UserStats>> Handle(NewProfileStats request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<UserStats>.Error(ErrorCode.ValidationError, errors);
            }

            var profile = await profileRepo.GetQueryable().AsNoTracking()
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return ApiResponse<UserStats>.Error(ErrorCode.NotFound, "User profile not found");



            var followers = followRepo.GetQueryable().AsNoTracking()
                .Where(f => f.FollowingId == profile.UserId)
                .Count();
            var folowings = followRepo.GetQueryable().AsNoTracking()
            .Where(f => f.FollowerId == profile.UserId).Count();

            var posts = postRepo.GetQueryable()
                                   .AsNoTracking()
                                   .Where(p => p.AuthorId == profile.Id)
                                 .Count();

            var UserStats = new UserStats()
            {
                NoOfFollowers = followers,
                NoOfFollowing = folowings,
                NoOfPosts = posts
            };

            return ApiResponse<UserStats>.Ok(UserStats);
        }
    }


}
