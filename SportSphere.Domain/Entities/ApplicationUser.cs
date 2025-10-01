using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSphere.Domain.Enums;

namespace SportSphere.Domain.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public UserRoles Role { get; set; }
        public string? ProfileImage { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public GenderType Gender { get; set; }

        public SocialProfile SocialProfile { get; set; } = null!;

        public ICollection<MediaFolder> MediaFolders { get; set; } = new List<MediaFolder>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<UserBlock> Blocker { get; set; } = new List<UserBlock>();
        public ICollection<UserBlock> Blocked { get; set; } = new List<UserBlock>();


        public ICollection<Report> ReportsMade { get; set; } = new List<Report>();
        public ICollection<Report> ReportsReceived { get; set; } = new List<Report>();

        // BaseEntity props
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted => DeletedAt.HasValue;
    }
}
