using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

namespace SportSphere.Domain.Entities
{
    [Index(nameof(UserId), IsUnique = true)]

    public class SocialProfile : BaseEntity
    {
        public string UserId { get; set; }
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    }
}
