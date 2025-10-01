using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.Domain.Common;
using SportSphere.Application.Features.Posts.Dto;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.Posts.Queries
{
    public class GetPostlikesQuery : IRequest<PagedResult<FollowerVm>>
    {
        public long PostId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class GetPostlikesQueryValidator : AbstractValidator<GetPostlikesQuery>
    {
        public GetPostlikesQueryValidator()
        {
            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("PostId is required");
        }
    }
    public class GetPostlikesQueryHandler : IRequestHandler<GetPostlikesQuery, PagedResult<FollowerVm>>
    {
        private readonly IGenericRepository<PostLike> LikeCommenttRepo;

        public GetPostlikesQueryHandler(IGenericRepository<PostLike> LikeCommenttRepo)
        {
            this.LikeCommenttRepo = LikeCommenttRepo;
        }

        public async Task<PagedResult<FollowerVm>> Handle(GetPostlikesQuery request, CancellationToken cancellationToken)
        {
            var comments = await LikeCommenttRepo.GetQueryable().AsNoTracking()
                .Where(c => c.PostId == request.PostId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
                .Select(c => new FollowerVm
                {
                    Bio = c.User.Bio,
                    ProfileId = c.User.Id,
                    UserID = c.User.User.Id,
                    FullName = c.User.User.FullName,
                    ProfilePictureUrl = $"/api/Media/mediaPreview?mediaId={c.User.User.ProfileImage}"
                })
                .ToListAsync(cancellationToken);
            var totalCount = comments.Count();
            return new PagedResult<FollowerVm>
            {
                Items = comments,
                PageIndex = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }


    }


}
