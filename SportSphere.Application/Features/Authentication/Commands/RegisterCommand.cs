using MagiXSquad.Application.Authentication.DTOs;
using MagiXSquad.Domain.Interfaces.Services;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Authentication.Commands
{
    public class RegisterCommand : IRequest<ApiResponse<RegisterRetDto>>
    {
        public RegisterDTO RegisterData { get; set; } = null!;
    }
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator(IValidator<RegisterDTO> dtoValidator)
        {
            RuleFor(x => x.RegisterData)
              .NotNull().WithMessage("RegisterData is required").SetValidator(dtoValidator);

        }
    }
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<RegisterRetDto>>
    {
        private readonly IEmailService emailService;
        private readonly IValidator<RegisterCommand> validator;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IEmailService emailService, IValidator<RegisterCommand> validator)
        {
            this.roleManager = roleManager;
            this.context = context;
            this.userManager = userManager;
            this.emailService = emailService;
            this.validator = validator;
        }

        public async Task<ApiResponse<RegisterRetDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));

                return ApiResponse<RegisterRetDto>.Error(ErrorCode.ValidationError, errors);

            }


            var existingUser = await userManager.FindByEmailAsync(request.RegisterData.Email);
            if (existingUser != null)
            {
                return ApiResponse<RegisterRetDto>.Error(ErrorCode.Conflict, "User with this email already exists");
            }
            var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == request.RegisterData.Role.ToString());
            if (role == null)
            {
                return ApiResponse<RegisterRetDto>.Error(ErrorCode.BadRequest, "Invalid role specified");
            }

            // Step 2: Create the user
            var user = new ApplicationUser
            {
                UserName = request.RegisterData.Email,
                Role = request.RegisterData.Role,
                Email = request.RegisterData.Email,
                FullName = request.RegisterData.FullName,
                Gender = request.RegisterData.Gender,
                DateOfBirth = request.RegisterData.DateOfBirth,
                PhoneNumber = request.RegisterData.PhoneNumber,
                IsActive = false,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };




            var createResult = await userManager.CreateAsync(user, request.RegisterData.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return ApiResponse<RegisterRetDto>.Error(ErrorCode.BadRequest, $"Registration failed: {errors}");
            }

            try
            {
                string roleName = role.Name;


                await userManager.AddToRoleAsync(user, roleName);



                await context.SaveChangesAsync();



                var response = new RegisterRetDto
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    Role = user.Role,
                    Message = "Registration successful. Please check your email to verify your account.",
                };

                return ApiResponse<RegisterRetDto>.Ok(response);
            }
            catch (Exception ex)
            {
                // Cleanup: Remove the user if post-creation steps fail
                await userManager.DeleteAsync(user);
                await context.SaveChangesAsync();

                return ApiResponse<RegisterRetDto>.Error(ErrorCode.InternalServerError, $"Registration failed due to an internal error.\nError : {ex.Message.ToString()}");
            }
        }
    }

}