using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Profile.Commands
{
    public class EditProfileCommand : IRequest<ApiResponse<bool>>
    {
        public EditProfileDto Profile { get; set; } = null!;
    }
    public class EditProfileCommandValidator : AbstractValidator<EditProfileCommand>
    {
        public EditProfileCommandValidator()
        {
            RuleFor(x => x.Profile).NotEmpty().WithMessage("Profile data is required");

        }
    }
    public class EditProfileHandler : IRequestHandler<EditProfileCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EditProfileHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<bool>> Handle(EditProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string? currentUserId = httpContextAccessor.HttpContext?.User?
    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You must login first");
                }

                var user = await _context.Users
                    .Include(u => u.SocialProfile)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

                if (user == null)
                { return ApiResponse<bool>.Error(ErrorCode.NotFound, "User not found"); }

                //var currentUser = _userManager.GetUserId;
                //if (!currentUser.Equals(currentUserId))
                //{ return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You are not authorized"); }

                //.Equals(request.UserId);

                var profile = request.Profile;
                user.FullName = profile.FullName ?? user.FullName;
                user.DateOfBirth = profile.DateOfBirth ?? user.DateOfBirth;
                user.Gender = profile.Gender ?? user.Gender;
                user.PhoneNumber = profile.PhoneNumber ?? user.PhoneNumber;
                user.ProfileImage = profile.ProfileImage ?? user.ProfileImage;

                var socialProfile = user.SocialProfile;
                if (socialProfile != null)
                {
                    socialProfile.Bio = profile.Bio ?? socialProfile.Bio;
                    socialProfile.Country = profile.Country ?? socialProfile.Country;
                    socialProfile.City = profile.City ?? socialProfile.City;

                    if (socialProfile is AthleteProfile athleteProfile)
                    {
                        athleteProfile.Sport = profile.Sport ?? athleteProfile.Sport;
                        athleteProfile.Position = profile.Position ?? athleteProfile.Position;
                        athleteProfile.HeightCm = profile.HeightCm ?? athleteProfile.HeightCm;
                        athleteProfile.WeightKg = profile.WeightKg ?? athleteProfile.WeightKg;
                    }
                }

                user.UpdatedAt = DateTime.UtcNow;


                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Ok(true, "Profile updated successfully");
            }
            catch (Exception)
            {
                return ApiResponse<bool>.Error(ErrorCode.UnknownError, "Unexpected error occurred");
            }
        }
    }
}
