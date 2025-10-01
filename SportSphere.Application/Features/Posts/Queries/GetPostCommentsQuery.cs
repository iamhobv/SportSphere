using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.Domain.Common;
using Microsoft.Extensions.Hosting;
using SportSphere.Application.Features.Posts.Dto;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SportSphere.Application.Features.Posts.Queries
{
    public class GetPostCommentsQuery : IRequest<PagedResult<PostCommentsVm>>
    {
        public long PostId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class GetPostCommentsQueryValidator : AbstractValidator<GetPostCommentsQuery>
    {
        public GetPostCommentsQueryValidator()
        {
            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("PostId is required");
        }
    }
    public class GetPostCommentsQueryHandler : IRequestHandler<GetPostCommentsQuery, PagedResult<PostCommentsVm>>
    {
        private readonly IGenericRepository<PostComment> PostCommenttRepo;

        public GetPostCommentsQueryHandler(IGenericRepository<PostComment> PostCommenttRepo)
        {
            this.PostCommenttRepo = PostCommenttRepo;
        }

        public async Task<PagedResult<PostCommentsVm>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
        {
            var comments = await PostCommenttRepo.GetQueryable().AsNoTracking().Include(c => c.User).ThenInclude(a => a.User)
                .Where(c => c.PostId == request.PostId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
                .Select(c => new PostCommentsVm
                {
                    CommentId = c.Id,
                    Content = c.Content,
                    AuthorName = c.User.User.FullName,
                    CreatedAt = c.CreatedAt,
                    AutherProfileImageUrl = $"/api/Media/mediaPreview?mediaId={c.User.User.ProfileImage}"
                })
                .ToListAsync(cancellationToken);
            var totalCount = comments.Count();
            return new PagedResult<PostCommentsVm>
            {
                Items = comments,
                PageIndex = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
