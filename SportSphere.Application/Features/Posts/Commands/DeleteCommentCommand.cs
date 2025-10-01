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
    public class DeleteCommentCommand : IRequest<ApiResponse<bool>>
    {
        public long CommentId { get; set; }
        public long ProfileId { get; set; } //current user profile id
    }
    public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentCommandValidator()
        {
            RuleFor(x => x.CommentId)
                .GreaterThan(0).WithMessage("CommentId is required");

            RuleFor(x => x.ProfileId)
                .GreaterThan(0).WithMessage("ProfileId is required");
        }
    }

    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, ApiResponse<bool>>
    {
        private readonly IValidator<DeleteCommentCommand> _validator;
        private readonly IGenericRepository<PostComment> postRepo;

        public DeleteCommentHandler(IValidator<DeleteCommentCommand> validator, IGenericRepository<PostComment> postRepo)
        {
            _validator = validator;
            this.postRepo = postRepo;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);
            }

            var comment = await postRepo
                .GetFilteredFirstOrDefaultAsync(c => c.Id == request.CommentId);

            if (comment == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Comment not found");

            if (comment.UserId != request.ProfileId)
                return ApiResponse<bool>.Error(ErrorCode.Forbidden, "You cannot delete this comment");

            postRepo.ActualDelete(comment);
            await postRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Comment deleted successfully");
        }
    }

}
