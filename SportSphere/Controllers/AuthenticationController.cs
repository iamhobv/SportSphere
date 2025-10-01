using System.Security.Claims;
using MagiXSquad.Application.Authentication.DTOs;
using Microsoft.AspNetCore.RateLimiting;
using SportSphere.Application.Features.Authentication.Commands;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Application.Features.Authentication.Orhcesterators;

namespace MagiXSquad.WebApi.Controllers
{
    [EnableRateLimiting("DefaultPolicy")]

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ApiResponse<RegisterRetDto>> Register([FromForm] RegisterOrchesteratorDTO registerDto)
        {

            //var result = await _mediator.Send(new RegisterCommand { RegisterData = registerDto });
            //return result;

            var result = await _mediator.Send(new RegisterOrchesterator { RegisterOrchesteratorData = registerDto });
            return result;
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] VerifyEmailDTO verifyEmailDto)
        {
            var command = new VerifyEmailCommand { VerificationData = verifyEmailDto };
            var result = await _mediator.Send(command);
            var html = result.Success
       ? "<html><body><h2>Email verified successfully!</h2><p>You can close this tab and go back to login page.</p></body></html>"
       : "<html><body><h2>Email verification failed!</h2><p>Please try again or contact support.</p></body></html>";

            return Content(html, "text/html");
            //return await _mediator.Send(command);
        }

        [HttpPost("send-email-verification")]
        public async Task<ApiResponse<bool>> SendEmailVerification([FromBody] SendEmailVerifictaionDto VerifictaionDto)
        {
            var command = new EmailVerificationCommand { Email = VerifictaionDto.Email };
            return await _mediator.Send(command);
        }

        [HttpPost("login")]
        public async Task<ApiResponse<LoginRetDTO>> Login([FromBody] LoginDTO loginDto)
        {
            var command = new LoginCommand { LoginData = loginDto };
            return await _mediator.Send(command);
        }

        [HttpPost("resend-email-verification")]
        public async Task<ApiResponse<bool>> ResendEmailVerification([FromBody] ResendVerificationDTO dto)
        {
            var command = new ResendVerificationCommand { Email = dto.Email };
            return await _mediator.Send(command);
        }

        [HttpPost("forgot-password")]
        public async Task<ApiResponse<bool>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var command = new ForgotPasswordCommand { ForgotPasswordData = forgotPasswordDto };
            return await _mediator.Send(command);
        }

        [HttpPost("reset-password")]
        public async Task<ApiResponse<bool>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var command = new ResetPasswordCommand { ResetPasswordData = resetPasswordDto };
            return await _mediator.Send(command);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ApiResponse<bool>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {

            var command = new ChangePasswordCommand
            {
                ChangePasswordData = changePasswordDto
            };

            return await _mediator.Send(command);
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<ApiResponse<bool>> Logout()
        {

            var command = new LogoutCommand
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty
            };

            return await _mediator.Send(command);
        }

        //[Authorize]
        //[HttpPost("deactivate")]
        //public async Task<ApiResponse<bool>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        //{

        //    var command = new ChangePasswordCommand
        //    {
        //        ChangePasswordData = changePasswordDto
        //    };

        //    return await _mediator.Send(command);
        //}
    }
}