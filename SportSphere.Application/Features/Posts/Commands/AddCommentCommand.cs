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

namespace SportSphere.Application.Features.Posts.Commands
{
    public class AddCommentCommand : IRequest<ApiResponse<bool>>
    {
        public AddCommentDto Comment { get; set; } = null!;
    }

    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            RuleFor(x => x.Comment.ProfileId)
                .GreaterThan(0).WithMessage("ProfileId is required");

            RuleFor(x => x.Comment.PostId)
                .GreaterThan(0).WithMessage("PostId is required");

            RuleFor(x => x.Comment.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");
        }
    }

    public class AddCommentHandler : IRequestHandler<AddCommentCommand, ApiResponse<bool>>
    {
        private readonly IValidator<AddCommentCommand> _validator;
        private readonly IGenericRepository<PostComment> commentRepo;
        private readonly IGenericRepository<Post> postRepo;
        private readonly IGenericRepository<SocialProfile> profileRepo;

        public AddCommentHandler(IValidator<AddCommentCommand> validator, IGenericRepository<PostComment> commentRepo, IGenericRepository<Post> postRepo, IGenericRepository<SocialProfile> profileRepo)
        {
            _validator = validator;
            this.commentRepo = commentRepo;
            this.postRepo = postRepo;
            this.profileRepo = profileRepo;
        }

        public async Task<ApiResponse<bool>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);
            }

            var post = await postRepo.GetFilteredFirstOrDefaultAsync(p => p.Id == request.Comment.PostId);

            if (post == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Post not found");

            var profile = await profileRepo.GetFilteredFirstOrDefaultAsync(sp => sp.Id == request.Comment.ProfileId);

            if (profile == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Profile not found");


            var comment = new PostComment
            {
                PostId = request.Comment.PostId,
                UserId = request.Comment.ProfileId,
                Content = request.Comment.Content,
                CreatedAt = DateTime.UtcNow
            };

            await commentRepo.AddAsync(comment);
            await commentRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Comment added successfully");
        }
    }

}
