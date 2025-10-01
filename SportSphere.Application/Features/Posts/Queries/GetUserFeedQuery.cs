using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Posts.Queries
{
    public class GetUserFeedQuery : IRequest<ApiResponse<List<PostVm>>>
    {
        public string CurrentUserId { get; set; } = null!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
    public class GetUserFeedQueryValidator : AbstractValidator<GetUserFeedQuery>
    {
        public GetUserFeedQueryValidator()
        {
            RuleFor(x => x.CurrentUserId).NotEmpty().WithMessage("CurrentUserId is required");
        }
    }
    public class GetUserFeedHandler : IRequestHandler<GetUserFeedQuery, ApiResponse<List<PostVm>>>
    {
        private readonly IValidator<GetUserFeedQuery> _validator;
        private readonly IGenericRepository<SocialProfile> profileRepo;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IGenericRepository<Follow> followRepo;
        private readonly IGenericRepository<UserBlock> blockRepo;
        private readonly IGenericRepository<Post> postRepo;

        public GetUserFeedHandler(IValidator<GetUserFeedQuery> validator, IGenericRepository<SocialProfile> profileRepo, UserManager<ApplicationUser> userManager, IGenericRepository<Follow> followRepo
            , IGenericRepository<UserBlock> blockRepo, IGenericRepository<Post> postRepo)
        {
            _validator = validator;
            this.profileRepo = profileRepo;
            this.userManager = userManager;
            this.followRepo = followRepo;
            this.blockRepo = blockRepo;
            this.postRepo = postRepo;
        }

        public async Task<ApiResponse<List<PostVm>>> Handle(GetUserFeedQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<List<PostVm>>.Error(ErrorCode.ValidationError, errors);
            }

            // get current user profile
            var profile = await profileRepo.GetQueryable().AsNoTracking()
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.UserId == request.CurrentUserId, cancellationToken);

            if (profile == null)
                return ApiResponse<List<PostVm>>.Error(ErrorCode.NotFound, "User profile not found");

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

            if (!visibleFollowingIds.Any())
                return ApiResponse<List<PostVm>>.Ok(new List<PostVm>(), "No following users");

            var posts = await postRepo.GetQueryable()
                .AsNoTracking()
                .Where(p => visibleFollowingIds.Contains(p.Author.UserId))
                .OrderByDescending(p => p.CreatedAt)
                    .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
                .Select(post => new PostVm
                {
                    PostId = post.Id,
                    Content = post.Content,
                    Caption = post.Caption,
                    CreatedAt = post.CreatedAt,
                    AuthorName = post.Author.User.FullName,
                    LikesCount = post.Likes.Count(),
                    CommentsCount = post.Comments.Count(),
                    Bio = post.Author.Bio,
                    ProfilePictureUrl = $"/api/Media/mediaPreview?mediaId={post.Author.User.ProfileImage}",
                    IsILiked = post.Likes.Any(l => l.UserId == profile.Id),
                    UserId = post.Author.User.Id,
                    MediaFiles = post.MediaFiles.Select(m => new MediaVm
                    {
                        MediaId = m.Id,
                        FileName = m.FileName,
                        ContentType = m.ContentType,
                        Url = $"/api/Media/mediaPreview?mediaId={m.Id}"
                    }).ToList() ?? new()
                }).ToListAsync();


            return ApiResponse<List<PostVm>>.Ok(posts);
        }
    }

}
