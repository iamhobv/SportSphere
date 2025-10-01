using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Application.Features.Profile.DTOs
{
    public class AddProfileDto
    {
        public string UserId { get; set; } = null!;

        public string? Bio { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        public string? Sport { get; set; }
        public string? Position { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? WeightKg { get; set; }
    }

}
