using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Lookups.Cities.DTO;
using SportSphere.Domain.Entities;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Lookups.Cities.Queries
{
    public class GetCityByCountyId : IRequest<ApiResponse<List<GetCitiesDTO>>>
    {
        public int CountyId { get; set; }
    }
    public class GetCityByCountyIdValidator : AbstractValidator<GetCityByCountyId>
    {
        public GetCityByCountyIdValidator()
        {
            RuleFor(a => a.CountyId).NotEmpty().WithMessage("County is required");
        }
    }
    public class GetCityByCountyIdHandler : IRequestHandler<GetCityByCountyId, ApiResponse<List<GetCitiesDTO>>>
    {
        private readonly ApplicationDbContext context;

        public GetCityByCountyIdHandler(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<ApiResponse<List<GetCitiesDTO>>> Handle(GetCityByCountyId request, CancellationToken cancellationToken)
        {
            var cities = await context.Cities
                   .Where(c => c.CountryId == request.CountyId)
                   .OrderBy(c => c.Name)
                   .Select(c => new GetCitiesDTO { CityId = c.CityId, Name = c.Name })
                   .ToListAsync();
            return ApiResponse<List<GetCitiesDTO>>.Ok(cities);
        }
    }
}
