using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Posts.Commands
{
    public class LikePostCommand : IRequest<ApiResponse<bool>>
    {
        public long ProfileId { get; set; }
        public long PostId { get; set; }
    }
    public class LikePostCommandValidator : AbstractValidator<LikePostCommand>
    {
        public LikePostCommandValidator()
        {
            RuleFor(x => x.ProfileId).GreaterThan(0).WithMessage("ProfileId is required");
            RuleFor(x => x.PostId).GreaterThan(0).WithMessage("PostId must be valid");
        }
    }
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand, ApiResponse<bool>>
    {
        private readonly IValidator<LikePostCommand> _validator;
        private readonly IGenericRepository<PostLike> likeRepo;
        private readonly IGenericRepository<Post> postRepo;
        private readonly IGenericRepository<SocialProfile> profileRepo;

        public LikePostCommandHandler(IValidator<LikePostCommand> validator, IGenericRepository<PostLike> likeRepo, IGenericRepository<Post> postRepo, IGenericRepository<SocialProfile> profileRepo)
        {
            _validator = validator;
            this.likeRepo = likeRepo;
            this.postRepo = postRepo;
            this.profileRepo = profileRepo;
        }

        public async Task<ApiResponse<bool>> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);
            }

            var post = await postRepo.GetQueryable()
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

            if (post == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Post not found");

            var CurrentUserProfile = await profileRepo.GetQueryable()
                .FirstOrDefaultAsync(sp => sp.Id == request.ProfileId, cancellationToken);

            if (CurrentUserProfile == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Current User not found");

            var alreadyLiked = post.Likes.Any(l => l.UserId == request.ProfileId);
            if (alreadyLiked)
                return ApiResponse<bool>.Error(ErrorCode.Conflict, "Current User already liked this post");

            var like = new PostLike
            {
                UserId = request.ProfileId,  // SocialProfile.Id - mistake in naming
                PostId = request.PostId,
                CreatedAt = DateTime.UtcNow
            };

            await likeRepo.AddAsync(like);
            await likeRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Post liked successfully");
        }
    }


}
