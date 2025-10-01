using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using SportSphere.Application.Features.Authentication.Commands;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Application.Features.Users.DTO;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Users.Queries
{
    public class SearchUsersQuery : IRequest<ApiResponse<searchUserResultDto>>
    {
        public SearchUsersCriteriaDTo Search { get; set; }

    }
    public class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
    {
        public SearchUsersQueryValidator()
        {
            RuleFor(x => x.Search.FullName)
                .MaximumLength(100).WithMessage("Full name must be at most 100 characters");
        }
    }
    public class SearchUsersHandler : IRequestHandler<SearchUsersQuery, ApiResponse<searchUserResultDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<SearchUsersQuery> validator;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SearchUsersHandler(ApplicationDbContext context, IValidator<SearchUsersQuery> validator, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.validator = validator;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<searchUserResultDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            var ValidateResult = await validator.ValidateAsync(request, cancellationToken);

            if (!ValidateResult.IsValid)
            {
                var errors = string.Join(", ", ValidateResult.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<searchUserResultDto>.Error(ErrorCode.ValidationError, errors);

            }
            string? currentUserId = httpContextAccessor.HttpContext?.User?
.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return ApiResponse<searchUserResultDto>.Error(ErrorCode.Unauthorized, "You must login first");
            }
            var query = _context.Users.AsNoTracking()
                .Include(u => u.SocialProfile)
                .Where(u => u.Role != UserRoles.SuperAdmin)
                .Where(u => u.Id != currentUserId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search.FullName))
            {
                var fullName = request.Search.FullName.Trim().ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(fullName));
            }

            if (request.Search.Gender.HasValue)
                query = query.Where(u => u.Gender == request.Search.Gender.Value);

            if (request.Search.Role.HasValue)
                query = query.Where(u => u.Role == request.Search.Role.Value);

            if (!string.IsNullOrWhiteSpace(request.Search.Sport))
                query = query.Where(u => (u.SocialProfile as AthleteProfile) != null &&
                                         ((AthleteProfile)u.SocialProfile).Sport.ToLower().Contains(request.Search.Sport.Trim().ToLower()));

            var totalCount = await query.CountAsync(cancellationToken);
            var skip = (request.Search.PageNumber - 1) * request.Search.PageSize;
            var users = await query
                   .Skip(skip)
                   .Take(request.Search.PageSize)
                   .Where(u => u.SocialProfile != null && (u.SocialProfile as AthleteProfile) != null)
                   .Select(u => new SearchUsersDto
                   {
                       UserId = u.Id,
                       FullName = u.FullName,
                       ProfileImage = $"/api/Media/mediaPreview?mediaId={u.ProfileImage}",
                       Bio = u.SocialProfile.Bio,
                       Role = u.Role.ToString(),
                       Sport = ((u.SocialProfile as AthleteProfile) != null ? ((AthleteProfile)u.SocialProfile).Sport : null)
                   })
                   .ToListAsync(cancellationToken);



            var result = new searchUserResultDto()
            {
                SearchUsersList = users,
                TotalCount = totalCount
            };
            return ApiResponse<searchUserResultDto>.Ok(result);
        }
    }
}
