using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Application.Features.Posts.Dto
{
    public class PostCommentsVm
    {
        public long CommentId { get; set; }
        public string Content { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string AutherProfileImageUrl { get; set; }
    }
}
