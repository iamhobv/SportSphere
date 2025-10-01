using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportSphere.Application.Features.Profile.Commands;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Application.Features.Profile.Queries;

namespace SportSphere.webAPI.Controllers
{
    [EnableRateLimiting("DefaultPolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProfileController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("upload-profile-picture")]
        public async Task<ApiResponse<long>> UploadProfilePicture([FromForm] UploadProfilePictureDto dto)
        {
            var command = new UploadProfilePictureCommand
            {
                File = dto.File
            };

            var mediaId = await mediator.Send(command);

            return mediaId;
        }
        [HttpPost("{userId}/upload-default-profile-picture")]
        public async Task<ApiResponse<long>> UploadDefaultProfilePicture([FromRoute] string userId, [FromForm] UploadProfilePictureDto dto)
        {
            var command = new AdminUploadDefaultProfilePhotoCommand
            {
                UserId = userId,
                File = dto.File
            };

            var mediaId = await mediator.Send(command);

            return mediaId;
        }

        [HttpGet("{userId}/profile-picture")]
        public async Task<IActionResult> GetProfilePicture([FromRoute] string userId)
        {
            var query = new GetProfilePictureQuery { UserId = userId };
            var fileStreamResult = await mediator.Send(query);

            return fileStreamResult;
        }

        [HttpGet("{PicId}/profile-pictureId")]
        public async Task<IActionResult> GetProfilePicture([FromRoute] long PicId)
        {
            var query = new GetProfilePictureByPicIdQuery { PictureId = PicId };
            var fileStreamResult = await mediator.Send(query);

            return fileStreamResult;
        }

        [HttpGet("GetProfile/{userId}")]
        public async Task<ApiResponse<UserProfileVm>> GetProfile(string userId)
        {
            var query = new GetUserProfileQuery { UserId = userId };
            var profile = await mediator.Send(query);
            return profile;
        }

        [HttpPut("EditProfile")]
        public async Task<ApiResponse<bool>> EditProfile([FromForm] EditProfileDto dto)
        {
            var command = new EditProfileCommand
            {
                Profile = dto
            };

            var response = await mediator.Send(command);
            return response;
        }


        [HttpPost("add-profile")]
        public async Task<ApiResponse<bool>> AddProfile([FromBody] AddProfileDto dto)
        {
            var command = new AddProfileCommand { Profile = dto };
            var response = await mediator.Send(command);
            return response;
        }

        [HttpGet("user/followers")]
        public async Task<ApiResponse<List<FollowerVm>>> GetFollowers([FromQuery] string userId)
        {
            var result = await mediator.Send(new GetFollowersListQuery { UserId = userId });
            return result;
        }
        [HttpGet("user/following")]
        public async Task<ApiResponse<List<FollowerVm>>> GetFollowing([FromQuery] string userId)
        {
            var result = await mediator.Send(new GetFollowingListQuery { UserId = userId });
            return result;
        }
        [HttpGet("GetHomeCardProfile/{userId}")]
        public async Task<ApiResponse<HomeProfileCard>> GetHomeCardProfile(string userId)
        {
            var query = new GetHomeCardProfileQuery { UserId = userId };
            var profile = await mediator.Send(query);
            return profile;
        }

        [HttpGet("user/following-for-home")]
        public async Task<ApiResponse<List<FollowerVm>>> GetFollowingForHome([FromQuery] string userId)
        {
            var result = await mediator.Send(new GetFollowingListQuery { UserId = userId });
            return result;
        }

        [HttpGet("GetProfileStats/{userId}")]
        public async Task<ApiResponse<UserStats>> GetProfileStats(string userId)
        {
            var query = new NewProfileStats { UserId = userId };
            var profile = await mediator.Send(query);
            return profile;
        }
        [HttpGet("user/poeple-you-may-know")]
        public async Task<ApiResponse<List<FollowerVm>>> PoepleYouMayKnow([FromQuery] string userId)
        {
            var result = await mediator.Send(new PeopleYouMayKnowQuery { UserId = userId });
            return result;
        }

    }
}
