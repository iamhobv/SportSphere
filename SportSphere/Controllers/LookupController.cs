using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportSphere.Application.Features.Lookups.Cities.DTO;
using SportSphere.Application.Features.Lookups.Cities.Queries;
using SportSphere.Application.Features.Lookups.Countries.DTO;
using SportSphere.Application.Features.Lookups.Countries.Queries;
using SportSphere.Application.Features.MediaFeatures.Queries;

namespace SportSphere.webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        public IMediator mediator { get; }
        public LookupController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("countries")]
        public async Task<ApiResponse<List<GetCountiesDTO>>> Countries()
        {
            var countries = await mediator.Send(new GetCountiesQuery());

            return countries;
        }

        [HttpGet("cities/{countyID:int}")]
        public async Task<ApiResponse<List<GetCitiesDTO>>> Cities(int countyID)
        {
            var cities = await mediator.Send(new GetCityByCountyId() { CountyId = countyID });

            return cities;
        }
    }
}
