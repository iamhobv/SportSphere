using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

namespace SportSphere.Domain.Entities
{
    [Index(nameof(FollowerId), IsUnique = false)]
    [Index(nameof(FollowingId), IsUnique = false)]

    public class Follow : BaseEntity
    {
        public string FollowerId { get; set; }
        public string FollowingId { get; set; }

        public ApplicationUser Follower { get; set; } = null!;
        public ApplicationUser Following { get; set; } = null!;
    }
}
