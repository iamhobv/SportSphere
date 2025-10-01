using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.Application.Authentication.DTOs;
using Microsoft.AspNetCore.Http;
using SportSphere.Application.Features.Profile.DTOs;

namespace SportSphere.Application.Features.Authentication.DTOs
{
    public class RegisterOrchesteratorDTO
    {
        public RegisterDTO RegisterData { get; set; } = null!;
        public AddProfileOrchestratorDto Profile { get; set; } = null!;
        public IFormFile File { get; set; } = null!;
    }
}
