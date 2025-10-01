using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Application.Features.Profile.Queries;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.Users.Queries
{


    public class BlockingListQuery : IRequest<ApiResponse<List<FollowerVm>>>
    {
        public string UserId { get; set; } = null!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

    }
    public class BlockingListQueryValidator : AbstractValidator<BlockingListQuery>
    {
        public BlockingListQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }


    public class BlockingListQueryHandler : IRequestHandler<BlockingListQuery, ApiResponse<List<FollowerVm>>>
    {
        private readonly IGenericRepository<SocialProfile> profileRepo;
        private readonly IValidator<BlockingListQuery> _validator;
        private readonly IGenericRepository<Follow> followRepo;
        private readonly IGenericRepository<UserBlock> blockRepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BlockingListQueryHandler(IGenericRepository<SocialProfile> profileRepo, IValidator<BlockingListQuery> validator
            , IGenericRepository<Follow> followRepo, IGenericRepository<UserBlock> blockRepo, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.profileRepo = profileRepo;
            _validator = validator;
            this.followRepo = followRepo;
            this.blockRepo = blockRepo;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<List<FollowerVm>>> Handle(BlockingListQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<List<FollowerVm>>.Error(ErrorCode.ValidationError, errors);
            }

            string? currentUserId = httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return ApiResponse<List<FollowerVm>>.Error(ErrorCode.Unauthorized, "You must login first");
            }


            var profile = await profileRepo.GetQueryable().AsNoTracking()
                .FirstOrDefaultAsync(sp => sp.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return ApiResponse<List<FollowerVm>>.Error(ErrorCode.NotFound, "User profile not found");

            var blockListIds = blockRepo.GetQueryable().AsNoTracking().Where(u => u.BlockerId == currentUserId).ToList();



            var blocklist = await userManager.Users
                             .AsNoTracking()
                             .Where(u => blockListIds.Select(b => b.BlockedId).Contains(u.Id))
                             .Skip((request.Page - 1) * request.PageSize)
                             .Take(request.PageSize)
                             .Select(x => new FollowerVm
                             {
                                 ProfileId = x.SocialProfile.Id,
                                 FullName = x.FullName,
                                 Bio = x.SocialProfile.Bio,
                                 ProfilePictureUrl = $"/api/Media/mediaPreview?mediaId={x.ProfileImage}",
                                 UserID = x.Id
                             })
                             .ToListAsync();



            return ApiResponse<List<FollowerVm>>.Ok(blocklist);
        }
    }

}
