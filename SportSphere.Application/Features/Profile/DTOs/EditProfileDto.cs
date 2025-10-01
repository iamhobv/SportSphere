using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Profile.DTOs
{
    public class EditProfileDto
    {
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderType? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }

        public string? Bio { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        public string? Sport { get; set; }
        public string? Position { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? WeightKg { get; set; }
    }

}
