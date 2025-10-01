using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Profile.Commands
{
    public class AddProfileCommand : IRequest<ApiResponse<bool>>
    {
        public AddProfileDto Profile { get; set; } = null!;
    }
    public class AddProfileCommandValidator : AbstractValidator<AddProfileCommand>
    {
        public AddProfileCommandValidator()
        {
            RuleFor(x => x.Profile.UserId).NotEmpty().WithMessage("UserId is required");

        }
    }
    public class AddProfileHandler : IRequestHandler<AddProfileCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _context;

        public AddProfileHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<bool>> Handle(AddProfileCommand request, CancellationToken cancellationToken)
        {
            var profileDto = request.Profile;

            var user = await _context.Users
                .Include(u => u.SocialProfile)
                .FirstOrDefaultAsync(u => u.Id == profileDto.UserId, cancellationToken);

            if (user == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "User not found");

            if (user.SocialProfile != null)
                return ApiResponse<bool>.Error(ErrorCode.Conflict, "Profile already exists for this user");

            var IsAthlete = user.Role;
            SocialProfile profile = new SocialProfile();
            if (IsAthlete == UserRoles.Athlete)
            {
                profile = new AthleteProfile
                {
                    UserId = user.Id,
                    User = user,
                    Sport = profileDto.Sport ?? string.Empty,
                    Position = profileDto.Position,
                    HeightCm = profileDto.HeightCm,
                    WeightKg = profileDto.WeightKg
                };
            }
            else
            {
                profile = new SocialProfile
                {
                    UserId = user.Id,
                    User = user
                };
            }

            profile.Bio = profileDto.Bio;
            profile.Country = profileDto.Country;
            profile.City = profileDto.City;

            _context.SocialProfiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);



            return ApiResponse<bool>.Ok(true, "Profile created successfully");
        }
    }
}
