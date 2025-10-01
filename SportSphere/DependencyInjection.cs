using FluentValidation;

using MagiXSquad.Domain.Interfaces;

using MagiXSquad.Infrastructure.Services.Implementation;


namespace MagiXSquad.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {






            services.AddScoped<IFileService, FileService>();



            #region Cross-Origin(AllowAll)

            /********************** Cross-Origin Resource Sharing (CORS) ********************/

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            #endregion

            return services;
        }
    }
}