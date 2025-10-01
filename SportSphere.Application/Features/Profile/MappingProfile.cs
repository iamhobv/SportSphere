using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSphere.Application.Features.Profile.DTOs;

namespace SportSphere.Application.Features.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<AddProfileOrchestratorDto, AddProfileDto>()
          .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
