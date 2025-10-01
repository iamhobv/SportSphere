using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Application.Features.Users.DTO
{
    public class searchUserResultDto
    {
        public int TotalCount { get; set; }
        public List<SearchUsersDto> SearchUsersList { get; set; }
    }
    public class SearchUsersDto
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfileImage { get; set; }
        public string? Bio { get; set; }
        public string? Role { get; set; }
        public string? Sport { get; set; }
    }
}
