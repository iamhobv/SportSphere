using System.ComponentModel.DataAnnotations;
using System.Text;
using MagiXSquad.Application.Authentication.DTOs;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class LoginCommand : IRequest<ApiResponse<LoginRetDTO>>
    {
        public LoginDTO LoginData { get; set; } = null!;
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.LoginData.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format. Please enter a valid email like user@example.com");

            RuleFor(x => x.LoginData.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 8 characters");
        }
    }


    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginRetDTO>>
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IValidator<LoginCommand> validator;

        // we don't need this for manual password checking for now
        public LoginCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager, IValidator<LoginCommand> validator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.validator = validator;
            _configuration = configuration;
        }

        public async Task<ApiResponse<LoginRetDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var ValidateResult = await validator.ValidateAsync(request, cancellationToken);

            if (!ValidateResult.IsValid)
            {
                var errors = string.Join(", ", ValidateResult.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<LoginRetDTO>.Error(ErrorCode.ValidationError, errors);

            }
            var user = await _userManager.FindByEmailAsync(request.LoginData.Email);

            if (user == null)
            {
                return ApiResponse<LoginRetDTO>.Error(ErrorCode.Unauthorized, "Invalid email or password");
            }

            if (!user.IsActive)
            {
                return ApiResponse<LoginRetDTO>.Error(ErrorCode.AccountNotActivated, "Your email address has not been verified. Please check your inbox for the verification email or request a new one. ");
            }
            else if (user.IsDeleted)
            {
                return ApiResponse<LoginRetDTO>.Error(ErrorCode.AccountDeactivated, "Your account has been deactivated.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginData.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var token = await GenerateJwtTokenAsync(user, request.LoginData.RememberMe);

                var loginResponse = new LoginRetDTO
                {
                    Token = token.Token,
                    Email = user.Email,
                    UserName = user.FullName,
                    UserId = user.Id,
                    Role = user.Role,
                    ExpiresAt = token.Expiration,

                };

                return ApiResponse<LoginRetDTO>.Ok(loginResponse);
            }
            else if (result.IsLockedOut)
            {
                return ApiResponse<LoginRetDTO>.Error(ErrorCode.TooManyFailedAttempts, "Account locked due to too many failed login attempts. Try again later.");

            }
            else
            {
                return ApiResponse<LoginRetDTO>.Error(ErrorCode.Unauthorized, "Invalid email or password");
            }
        }

        private async Task<(string Token, DateTime Expiration)> GenerateJwtTokenAsync(ApplicationUser user, bool rememberMe)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
            };

            // Add role claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _configuration["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured");
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var expires = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(12);

            var token = new JwtSecurityToken(
                //audience: _configuration["JWT:Audience"], // Add audience
                issuer: _configuration["JWT:Issuer"],
                expires: expires,
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }

}