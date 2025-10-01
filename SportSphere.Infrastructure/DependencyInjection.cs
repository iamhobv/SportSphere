

using Microsoft.AspNetCore.Builder.Extensions;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;
using SportSphere.Infrastructure.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportSphere.Domain.Services;
using MagiXSquad.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MagiXSquad.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            /*************************** Database & User Identity ***************************/
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("GlobalConnectionString"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;


                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;


                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            //services.AddScoped<IApplicationDbContext>(sp => (IApplicationDbContext)sp.GetRequiredService<ApplicationDbContext>());


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    var jwtKey = configuration["JWT:Key"];
    var key = Encoding.UTF8.GetBytes(jwtKey);

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["JWT:Issuer"],
        ValidAudience = configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

            /********************************************************************************/
            return services;
        }
    }
}