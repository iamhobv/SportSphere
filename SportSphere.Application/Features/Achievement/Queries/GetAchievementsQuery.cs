using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Achievement.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Achievement.Queries
{
    public class GetAchievementsQuery : IRequest<ApiResponse<List<GetAthleteAchievementDto>>>
    {
        public long AthleteProfileId { get; set; }
    }
    public class GetAchievementsQueryValidator : AbstractValidator<GetAchievementsQuery>
    {
        public GetAchievementsQueryValidator()
        {
            RuleFor(x => x.AthleteProfileId)
                .GreaterThan(0).WithMessage("AthleteProfileId is required");
        }
    }
    public class GetAchievementsQueryHandler : IRequestHandler<GetAchievementsQuery, ApiResponse<List<GetAthleteAchievementDto>>>
    {
        private readonly IValidator<GetAchievementsQuery> _validator;
        private readonly IGenericRepository<AthleteAchievement> achievementRepo;
        private readonly IGenericRepository<AthleteProfile> athleteProfileRepo;

        public GetAchievementsQueryHandler(IValidator<GetAchievementsQuery> validator, IGenericRepository<AthleteAchievement> AchievementRepo, IGenericRepository<AthleteProfile> AthleteProfileRepo)
        {
            _validator = validator;
            achievementRepo = AchievementRepo;
            athleteProfileRepo = AthleteProfileRepo;
        }

        public async Task<ApiResponse<List<GetAthleteAchievementDto>>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<List<GetAthleteAchievementDto>>.Error(ErrorCode.ValidationError, errors);
            }
            var athleteProfile = await athleteProfileRepo.GetFilteredFirstOrDefaultAsync(a => a.Id == request.AthleteProfileId);

            if (athleteProfile == null)
                return ApiResponse<List<GetAthleteAchievementDto>>.Error(ErrorCode.NotFound, "Athlete profile not found");

            var achievements = await achievementRepo.GetQueryable().AsNoTracking()
                .Where(a => a.AthleteId == request.AthleteProfileId)
                .OrderByDescending(a => a.Date)
                .Select(a => new GetAthleteAchievementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Date = a.Date
                })
                .ToListAsync();

            return ApiResponse<List<GetAthleteAchievementDto>>.Ok(achievements);
        }
    }
}
