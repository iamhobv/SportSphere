using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Posts.Dto;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Posts.Queries
{
    public class GetPostByIdQuery : IRequest<ApiResponse<PostDetailsVm>>
    {
        public long PostId { get; set; }
    }
    public class GetPostByIdQueryValidator : AbstractValidator<GetPostByIdQuery>
    {
        public GetPostByIdQueryValidator()
        {
            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("PostId is required");
        }
    }
    public class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, ApiResponse<PostDetailsVm>>
    {
        private readonly IValidator<GetPostByIdQuery> _validator;
        private readonly IGenericRepository<Post> postRepo;

        public GetPostByIdHandler(IValidator<GetPostByIdQuery> validator, IGenericRepository<Post> postRepo)
        {
            _validator = validator;
            this.postRepo = postRepo;
        }

        public async Task<ApiResponse<PostDetailsVm>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<PostDetailsVm>.Error(ErrorCode.ValidationError, errors);
            }

            var post = await postRepo.GetQueryable().AsNoTracking()
                .Include(p => p.Author).ThenInclude(a => a.User)
                .Include(p => p.MediaFiles)
                .Include(p => p.Comments).ThenInclude(c => c.User).ThenInclude(u => u.User)
                .Include(p => p.Likes).ThenInclude(l => l.User).ThenInclude(u => u.User)
                .FirstOrDefaultAsync(p => p.Id == request.PostId);

            if (post == null)
                return ApiResponse<PostDetailsVm>.Error(ErrorCode.NotFound, "Post not found");

            var result = new PostDetailsVm
            {
                UserId = post.Author.User.Id,

                PostId = post.Id,
                Content = post.Content,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt,
                AuthorName = post.Author.User.FullName,
                CommentsCount = post.Comments.Count,
                LikesCount = post.Likes.Count,
                MediaFiles = post.MediaFiles?.Select(m => $"/api/Media/mediaPreview?mediaId={m.Id}").ToList() ?? new(),
                Comments = post.Comments?.Select(c => new CommentVm
                {
                    CommentId = c.Id,
                    AuthorName = c.User.User.FullName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                }).ToList() ?? new(),
                Likes = post.Likes?.Select(l => new LikeVm
                {
                    UserId = l.User.Id,
                    UserName = l.User.User.FullName
                }).ToList() ?? new()
            };

            return ApiResponse<PostDetailsVm>.Ok(result);
        }
    }


}
