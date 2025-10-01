using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Application.Features.Posts.Dto
{
    public class AddCommentDto
    {
        public long ProfileId { get; set; }
        public long PostId { get; set; }
        public string Content { get; set; } = null!;
    }

}
