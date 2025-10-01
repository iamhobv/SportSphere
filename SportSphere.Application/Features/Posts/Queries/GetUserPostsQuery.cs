using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Mvc;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Application.Features.Profile.Queries;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Posts.Queries
{
    public class GetUserPostsQuery : IRequest<ApiResponse<List<PostVm>>>
    {
        public string UserId { get; set; } = null!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class GetUserPostsQueryValidator : AbstractValidator<GetUserPostsQuery>
    {
        public GetUserPostsQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }


    public class GetUserPostsHandler : IRequestHandler<GetUserPostsQuery, ApiResponse<List<PostVm>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<GetUserPostsQuery> _validator;
        private readonly IGenericRepository<SocialProfile> profileRepo;
        private readonly IGenericRepository<Post> postRepo;
        private readonly IMediator mediator;

        public GetUserPostsHandler(ApplicationDbContext context, IValidator<GetUserPostsQuery> validator,
            IGenericRepository<SocialProfile> profileRepo,
            IGenericRepository<Post> postRepo,
            IMediator mediator)
        {
            _context = context;
            _validator = validator;
            this.profileRepo = profileRepo;
            this.postRepo = postRepo;
            this.mediator = mediator;
        }

        public async Task<ApiResponse<List<PostVm>>> Handle(GetUserPostsQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<List<PostVm>>.Error(ErrorCode.ValidationError, errors);
            }

            var profile = await profileRepo.GetQueryable().AsNoTracking()
                        .Include(sp => sp.User)
                        .FirstOrDefaultAsync(sp => sp.UserId == request.UserId, cancellationToken);

            if (profile == null)
                return ApiResponse<List<PostVm>>.Error(ErrorCode.NotFound, "User profile not found");



            var posts = await postRepo.GetQueryable()
                        .AsNoTracking()
                        .Where(p => p.AuthorId == profile.Id)
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
                            MediaFiles = post.MediaFiles.Select(m => new MediaVm
                            {
                                MediaId = m.Id,
                                FileName = m.FileName,
                                ContentType = m.ContentType,
                                Url = $"/api/Media/mediaPreview?mediaId={m.Id}"
                            }).ToList() ?? new()
                        }).ToListAsync();


            //var posts = await postRepo.GetQueryable().AsNoTracking()
            //    .Include(p => p.MediaFiles)
            //    .Include(p => p.Likes)
            //    .Include(p => p.Comments)
            //    .Where(p => p.AuthorId == profile.Id)
            //    .OrderByDescending(p => p.CreatedAt)
            //    .ToListAsync();

            //if (posts.Count == 0)
            //{
            //    return ApiResponse<List<PostVm>>.Error(ErrorCode.NotFound, "User has no posts");

            //}


            //var result = posts.Select(post => new PostVm
            //{
            //    PostId = post.Id,
            //    Content = post.Content,
            //    Caption = post.Caption,
            //    CreatedAt = post.CreatedAt,
            //    AuthorName = profile.User.FullName,
            //    LikesCount = post.Likes?.Count ?? 0,
            //    CommentsCount = post.Comments?.Count ?? 0,
            //    MediaFiles = post.MediaFiles?.Select(m => new MediaVm
            //    {
            //        MediaId = m.Id,
            //        FileName = m.FileName,
            //        ContentType = m.ContentType,
            //        Url = $"/api/Media/mediaPreview?mediaId={m.Id}"
            //    }).ToList() ?? new()
            //}).ToList();


            return ApiResponse<List<PostVm>>.Ok(posts);
        }
    }
}
