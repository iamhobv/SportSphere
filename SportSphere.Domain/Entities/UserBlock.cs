using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Domain.Entities
{
    public class UserBlock : BaseEntity
    {
        public string BlockerId { get; set; } = null!;
        public string BlockedId { get; set; } = null!;

        public ApplicationUser Blocker { get; set; } = null!;
        public ApplicationUser Blocked { get; set; } = null!;
    }

}
