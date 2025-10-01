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

namespace SportSphere.Application.Features.Achievement.Commands
{
    public class AddAchievementCommand : IRequest<ApiResponse<bool>>
    {
        public AddAchievementCommandDTO AchievementData { get; set; }
    }
    public class AddAchievementCommandValidator : AbstractValidator<AddAchievementCommand>
    {
        public AddAchievementCommandValidator()
        {
            RuleFor(x => x.AchievementData.AthleteProfileId).GreaterThan(0).WithMessage("AthleteProfileId is required");
            RuleFor(x => x.AchievementData.Title).NotEmpty().MaximumLength(200).WithMessage("Title is required and max 200 chars");
            RuleFor(x => x.AchievementData.Date).LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Achievement date cannot be in the future");
        }
    }
    public class AddAchievementCommandHandler : IRequestHandler<AddAchievementCommand, ApiResponse<bool>>
    {
        private readonly IValidator<AddAchievementCommand> _validator;
        private readonly IGenericRepository<AthleteAchievement> achievementRepo;
        private readonly IGenericRepository<AthleteProfile> athleteProfileRepo;

        public AddAchievementCommandHandler(IValidator<AddAchievementCommand> validator, IGenericRepository<AthleteAchievement> AchievementRepo, IGenericRepository<AthleteProfile> AthleteProfileRepo)
        {
            _validator = validator;
            achievementRepo = AchievementRepo;
            athleteProfileRepo = AthleteProfileRepo;
        }

        public async Task<ApiResponse<bool>> Handle(AddAchievementCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);
            }

            var athleteProfile = await athleteProfileRepo.GetFilteredFirstOrDefaultAsync(a => a.Id == request.AchievementData.AthleteProfileId);

            if (athleteProfile == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Athlete profile not found");

            //var athleteAchivment = await achievementRepo.GetQueryable()
            //    .Include(a => a.Athlete)
            //    .FirstOrDefaultAsync(a => a.Athlete.Id == request.AchievementData.AthleteProfileId);

            //if (athleteAchivment?.Athlete == null)
            //    return ApiResponse<bool>.Error(ErrorCode.NotFound, "Athlete profile not found");

            var achievement = new AthleteAchievement
            {
                AthleteId = request.AchievementData.AthleteProfileId,
                Title = request.AchievementData.Title,
                Description = request.AchievementData.Description,
                Date = request.AchievementData.Date
            };

            await achievementRepo.AddAsync(achievement);

            await achievementRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Achievement added successfully");
        }
    }
}
