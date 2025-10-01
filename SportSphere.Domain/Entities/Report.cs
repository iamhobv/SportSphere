using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportSphere.Domain.Enums;

namespace SportSphere.Domain.Entities
{
    public class Report : BaseEntity
    {
        public string ReporterId { get; set; } = null!;
        public string? ReportedUserId { get; set; }
        public long? ReportedPostId { get; set; }
        public long? ReportedCommentId { get; set; }

        public string Reason { get; set; } = null!;
        public string? Details { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public ApplicationUser Reporter { get; set; } = null!;
        public ApplicationUser? ReportedUser { get; set; }
        public Post? ReportedPost { get; set; }
        public PostComment? ReportedComment { get; set; }
    }
}
