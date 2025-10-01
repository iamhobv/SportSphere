using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportSphere.Application.Features.Posts.Commands;
using SportSphere.Application.Features.Posts.Dto;
using SportSphere.Application.Features.Posts.Queries;

namespace SportSphere.webAPI.Controllers
{
    [EnableRateLimiting("DefaultPolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMediator mediator;

        public PostController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("write-post")]
        public async Task<ApiResponse<bool>> WritePost([FromForm] WritePostCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return result;
        }

        [HttpGet("users/posts")]
        public async Task<IActionResult> GetUserPosts(string UserId, int Page, int PageSize)
        {
            var query = new GetUserPostsQuery { UserId = UserId, PageSize = PageSize, Page = Page };
            var result = await mediator.Send(query);
            return Ok(result);
        }
        [HttpPost("like-post")]
        public async Task<ApiResponse<bool>> LikePost([FromQuery] long postId, [FromQuery] long profileId, CancellationToken cancellationToken)
        {
            var command = new LikePostCommand { PostId = postId, ProfileId = profileId };
            var result = await mediator.Send(command, cancellationToken);
            return result;
        }
        [HttpPost("unlike-post")]
        public async Task<ApiResponse<bool>> unLikePost([FromQuery] long postId, [FromQuery] long profileId, CancellationToken cancellationToken)
        {
            var command = new UnLikePostCommand { PostId = postId, ProfileId = profileId };
            var result = await mediator.Send(command, cancellationToken);
            return result;
        }
        [HttpPost("add-comment")]
        public async Task<ApiResponse<bool>> AddComment(AddCommentDto dto, CancellationToken cancellationToken)
        {
            var command = new AddCommentCommand { Comment = dto };

            var result = await mediator.Send(command, cancellationToken);
            return result;
        }

        [HttpDelete("delete-comment")]
        public async Task<ApiResponse<bool>> DeleteComment([FromQuery] long postId, [FromQuery] long commentId, [FromQuery] long profileId)
        {
            var command = new DeleteCommentCommand
            {
                CommentId = commentId,
                ProfileId = profileId
            };

            var result = await mediator.Send(command);
            return result;
        }


        [HttpGet("post")]
        public async Task<ApiResponse<PostDetailsVm>> GetPostById(long postId)
        {
            var query = new GetPostByIdQuery { PostId = postId };
            var result = await mediator.Send(query);
            return result;
        }
        [HttpGet("users/feed")]
        public async Task<IActionResult> GetUserFeed(string currentUserId, int Page, int PageSize)
        {
            var query = new GetUserFeedQuery { CurrentUserId = currentUserId, PageSize = PageSize, Page = Page };
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("posts/suggested")]
        public async Task<IActionResult> GetSuggestedPosts(string currentUserId, int Page, int PageSize)
        {
            var query = new GetSuggestedUsersPostsQuery { CurrentUserId = currentUserId, PageSize = PageSize, Page = Page };
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("post/{postId}/comments")]
        public async Task<IActionResult> GetPostComments(long postId, int Page, int PageSize)
        {
            var query = new GetPostCommentsQuery { PostId = postId, PageSize = PageSize, Page = Page };
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("post/{postId}/Likes")]
        public async Task<IActionResult> GetPostlikes(long postId, int Page, int PageSize)
        {
            var query = new GetPostlikesQuery { PostId = postId, PageSize = PageSize, Page = Page };
            var result = await mediator.Send(query);
            return Ok(result);
        }


    }
}
