using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Application.Features.Reports.DTO
{
    public class ReportContentCommandDTO
    {
        public string ReporterId { get; set; } = null!;
        public string? ReportedUserId { get; set; }
        public long? ReportedPostId { get; set; }
        public long? ReportedCommentId { get; set; }
        public string Reason { get; set; } = null!;
        public string? Details { get; set; }
    }
}
