using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportSphere.Application.Features.MediaFeatures.Queries;
using SportSphere.Application.Features.Profile.Queries;

namespace SportSphere.webAPI.Controllers
{
    //[EnableRateLimiting("DefaultPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediator mediator;

        public MediaController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet("mediaPreview")]
        public async Task<IActionResult> MediaPreview([FromQuery] long mediaId)
        {
            var query = new MediaPreviewQuery { PictureId = mediaId };
            var fileStreamResult = await mediator.Send(query);

            return fileStreamResult;
        }

    }
}
