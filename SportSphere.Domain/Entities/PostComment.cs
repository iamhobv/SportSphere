using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

namespace SportSphere.Domain.Entities
{
    [Index(nameof(PostId), IsUnique = false)]

    public class PostComment : BaseEntity
    {
        public long UserId { get; set; }
        public long PostId { get; set; }
        public string Content { get; set; } = null!;

        public SocialProfile User { get; set; } = null!;
        public Post Post { get; set; } = null!;
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
