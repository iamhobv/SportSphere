
using System.Text.Json.Serialization;
using AutoMapper;
using MagiXSquad.Application;
using MagiXSquad.Application.Services;
using MagiXSquad.Infrastructure;
using MagiXSquad.WebApi;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using SportSphere.webAPI.Middleware;
using System;
using SportSphere.Infrastructure.DataContext;
using SportSphere.Application.Services;

namespace SportSphere
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddWebApi();
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Serialize enums as strings instead of integers
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());






            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("DefaultPolicy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.RejectionStatusCode = 429;
            });




            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            //app.UseRateLimiter();

            //seed countries
            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    DbCountriesSeeder.SeedCountriesFromDictionary(
            //        db,
            //        Path.Combine(app.Environment.ContentRootPath, "Data", "countries.json")
            //    );
            //}

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            MapperServices.Mapper = app.Services.GetService<IMapper>();
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
