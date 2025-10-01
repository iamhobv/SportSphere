using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Application.Features.Posts.Dto
{
    public class PostDetailsVm
    {
        public long PostId { get; set; }
        public string Content { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; } = null!;

        public long LikesCount { get; set; }
        public long CommentsCount { get; set; }

        public List<string> MediaFiles { get; set; } = new(); // e.g. URLs to preview
        public List<CommentVm> Comments { get; set; } = new();
        public List<LikeVm> Likes { get; set; } = new();
    }

    public class CommentVm
    {
        public long CommentId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class LikeVm
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = null!;
    }

}
