using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Users.DTO
{
    public class SearchUsersCriteriaDTo
    {
        public string FullName { get; set; }
        public GenderType? Gender { get; set; }
        public UserRoles? Role { get; set; }
        public string? Sport { get; set; }
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
