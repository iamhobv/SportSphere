using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Profile.DTOs
{
    public class HomeProfileCard
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfileImage { get; set; }
        public string? ProfileImageURL { get; set; }
        public string? Bio { get; set; }
        public string? Sport { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public bool IsIFollow { get; set; }
        public bool IsIBlock { get; set; }
        public long? ProfileID { get; set; }
        public UserRoles Role { get; set; }

    }
}
