using MagiXSquad.Application.Authentication.DTOs;
using MagiXSquad.Application.Services;
using MagiXSquad.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using SportSphere.Application.Features.Authentication.Commands;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Application.Features.Users.Queries;
using SportSphere.Domain.Services;

namespace MagiXSquad.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            // Email sender
            services.Configure<SmtpOptions>(
                configuration.GetSection("SmtpOptions")
            );
            services.AddTransient<IEmailService, EmailService>();
            services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<VerifyEmailCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<EmailVerificationValidator>();

            services.AddValidatorsFromAssemblyContaining<ForgotPasswordValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<ResendVerificationVAlidator>();
            services.AddValidatorsFromAssemblyContaining<ResetPasswordValidator>();
            services.AddValidatorsFromAssemblyContaining<ResetPasswordDtoVAlidator>();
            services.AddValidatorsFromAssemblyContaining<SearchUsersQueryValidator>();
            //services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

            services.AddHttpContextAccessor();





            return services;
        }
    }
}