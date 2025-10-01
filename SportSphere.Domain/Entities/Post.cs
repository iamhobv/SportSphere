using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

namespace SportSphere.Domain.Entities
{
    [Index(nameof(AuthorId), IsUnique = false)]

    public class Post : BaseEntity
    {
        public long AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public string? Caption { get; set; }

        public SocialProfile Author { get; set; } = null!;
        public ICollection<PostLike>? Likes { get; set; }
        public ICollection<Media>? MediaFiles { get; set; }
        public ICollection<PostComment>? Comments { get; set; }
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
