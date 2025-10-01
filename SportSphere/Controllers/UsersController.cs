using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportSphere.Application.Features.Profile.Commands;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Application.Features.Profile.Queries;
using SportSphere.Application.Features.Users.Commands;
using SportSphere.Application.Features.Users.DTO;
using SportSphere.Application.Features.Users.Queries;

namespace SportSphere.webAPI.Controllers
{
    [EnableRateLimiting("DefaultPolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost("search-users")]
        public async Task<ApiResponse<searchUserResultDto>> SearchForUSers([FromForm] SearchUsersCriteriaDTo search)
        {
            var query = new SearchUsersQuery { Search = search };
            var fileStreamResult = await mediator.Send(query);

            return fileStreamResult;
        }

        [HttpPost("Follow")]
        public async Task<ApiResponse<bool>> Follow(string UserIdToFollow)
        {
            //var command = new FollowUserCommand { UserIdToFollow = UserIdToFollow };
            var response = await mediator.Send(new FollowUserCommand { UserIdToFollow = UserIdToFollow });
            return response;
        }
        [HttpPost("Unfollow")]
        public async Task<ApiResponse<bool>> Unfollow(string UserIdToUnfollow)
        {
            //var command = new FollowUserCommand { UserIdToFollow = UserIdToFollow };
            var response = await mediator.Send(new UnfollowUserCommand { UserIdToUnfollow = UserIdToUnfollow });
            return response;
        }
        [HttpPost("block")]
        public async Task<IActionResult> BlockUser(string UserIdToBlock)
        {
            var result = await mediator.Send(new BlockUserCommand { UserIdToBlock = UserIdToBlock });
            return Ok(result);
        }

        [HttpPost("unblock")]
        public async Task<IActionResult> UnblockUser(string UserIdUnblock)
        {
            var result = await mediator.Send(new UnblockUserCommand { UserIDtoUnBlocked = UserIdUnblock });
            return Ok(result);
        }
        [HttpGet("get-blocking-list")]
        public async Task<ApiResponse<List<FollowerVm>>> BlockingList(string UserId, int Page, int PageSize)
        {
            var result = await mediator.Send(new BlockingListQuery { UserId = UserId, PageSize = PageSize, Page = Page });
            return result;
        }
    }
}
