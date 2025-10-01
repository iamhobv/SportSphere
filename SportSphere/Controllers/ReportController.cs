using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SportSphere.Application.Features.Reports.Commands;
using SportSphere.Application.Features.Reports.DTO;

namespace SportSphere.webAPI.Controllers
{
    [EnableRateLimiting("DefaultPolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReportController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("add-report")]
        public async Task<IActionResult> Report([FromBody] ReportContentCommandDTO command)
        {
            var result = await mediator.Send(new ReportContentCommand() { ReportContentCommandData = command });
            return Ok(result);
        }

    }
}
