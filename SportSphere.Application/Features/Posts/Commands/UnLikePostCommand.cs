using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.Posts.Commands
{

    public class UnLikePostCommand : IRequest<ApiResponse<bool>>
    {
        public long ProfileId { get; set; }
        public long PostId { get; set; }
    }
    public class UnLikePostCommandValidator : AbstractValidator<UnLikePostCommand>
    {
        public UnLikePostCommandValidator()
        {
            RuleFor(x => x.ProfileId).GreaterThan(0).WithMessage("ProfileId is required");
            RuleFor(x => x.PostId).GreaterThan(0).WithMessage("PostId must be valid");
        }
    }

    public class UnLikePostCommandHandler : IRequestHandler<UnLikePostCommand, ApiResponse<bool>>
    {
        private readonly IValidator<UnLikePostCommand> _validator;
        private readonly IGenericRepository<PostLike> likeRepo;
        private readonly IGenericRepository<Post> postRepo;
        private readonly IGenericRepository<SocialProfile> profileRepo;

        public UnLikePostCommandHandler(IValidator<UnLikePostCommand> validator, IGenericRepository<PostLike> likeRepo, IGenericRepository<Post> postRepo, IGenericRepository<SocialProfile> profileRepo)
        {
            _validator = validator;
            this.likeRepo = likeRepo;
            this.postRepo = postRepo;
            this.profileRepo = profileRepo;
        }

        public async Task<ApiResponse<bool>> Handle(UnLikePostCommand request, CancellationToken cancellationToken)
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


            var like = await likeRepo.GetFilteredFirstOrDefaultAsync(l => l.UserId == request.ProfileId && l.PostId == request.PostId);

            if (like == null)
                return ApiResponse<bool>.Error(ErrorCode.Conflict, "Current User already not liked this post");

            likeRepo.ActualDelete(like);
            //await likeRepo.AddAsync(like);
            await likeRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Post unliked successfully");
        }
    }
}
