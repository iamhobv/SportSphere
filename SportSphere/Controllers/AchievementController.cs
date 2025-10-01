using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportSphere.Application.Features.Achievement.Commands;
using SportSphere.Application.Features.Achievement.DTOs;
using SportSphere.Application.Features.Achievement.Queries;

namespace SportSphere.webAPI.Controllers
{
    [EnableRateLimiting("DefaultPolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        public IMediator mediator { get; }
        public AchievementController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost("athletes/achievements")]
        public async Task<ApiResponse<bool>> AddAchievement(AddAchievementCommand command)
        {
            var result = await mediator.Send(command);
            return result;
        }
        [HttpPost("athletes/get-achievements")]
        public async Task<ApiResponse<List<GetAthleteAchievementDto>>> GetAchievements(GetAchievementsQuery query)
        {
            var result = await mediator.Send(query);
            return result;
        }


    }
}
