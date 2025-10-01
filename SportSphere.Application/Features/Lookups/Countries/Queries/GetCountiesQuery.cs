using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Lookups.Countries.DTO;
using SportSphere.Domain.Entities;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Lookups.Countries.Queries
{
    public class GetCountiesQuery : IRequest<ApiResponse<List<GetCountiesDTO>>>
    {
    }
    public class GetCountiesQueryHandler : IRequestHandler<GetCountiesQuery, ApiResponse<List<GetCountiesDTO>>>
    {
        private readonly ApplicationDbContext context;

        public GetCountiesQueryHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ApiResponse<List<GetCountiesDTO>>> Handle(GetCountiesQuery request, CancellationToken cancellationToken)
        {
            var countries = await context.Countries
                    .OrderBy(c => c.Name)
                    .Select(c => new GetCountiesDTO { CountryId = c.CountryId, Name = c.Name })
                    .ToListAsync();

            return ApiResponse<List<GetCountiesDTO>>.Ok(countries);
        }
    }

}
