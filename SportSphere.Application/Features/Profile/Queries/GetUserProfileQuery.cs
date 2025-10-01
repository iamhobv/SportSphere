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
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Profile.Queries
{
    public class GetUserProfileQuery : IRequest<ApiResponse<UserProfileVm>>
    {
        public string UserId { get; set; } = null!;
    }
    public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
    {
        public GetUserProfileQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }
    public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, ApiResponse<UserProfileVm>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GetUserProfileHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<UserProfileVm>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isCurrentUser = request.UserId == currentUserId;

                var user = await userManager.FindByIdAsync(request.UserId);

                if (user == null)
                    return ApiResponse<UserProfileVm>.Error(ErrorCode.NotFound, "User not found");


                var userData = context.Users.AsNoTracking().Where(u => u.Id == request.UserId)
                                 .Select(u => new UserProfileVm
                                 {
                                     UserId = u.Id,
                                     FullName = u.FullName,
                                     ProfileImage = u.ProfileImage,
                                     Bio = u.SocialProfile.Bio,
                                     Country = u.SocialProfile.Country,
                                     City = u.SocialProfile.City,
                                     Gender = u.Gender,
                                     DateOfBirth = u.DateOfBirth,
                                     PhoneNumber = u.PhoneNumber,
                                     Sport = (u.SocialProfile as AthleteProfile).Sport,
                                     Height = (u.SocialProfile as AthleteProfile).HeightCm,
                                     Weight = (u.SocialProfile as AthleteProfile).WeightKg,
                                     Psition = (u.SocialProfile as AthleteProfile).Position,
                                     Role = u.Role


                                 }).FirstOrDefault(u => u.UserId == request.UserId);





                return ApiResponse<UserProfileVm>.Ok(userData);
            }
            catch (Exception)
            {
                return ApiResponse<UserProfileVm>.Error(ErrorCode.UnknownError, "UnExpected error");


            }

        }
    }
}
