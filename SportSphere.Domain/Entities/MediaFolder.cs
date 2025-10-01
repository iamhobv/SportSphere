using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Domain.Entities
{
    public class MediaFolder : BaseEntity
    {
        public string FolderName { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Media> MediaFiles { get; set; } = new List<Media>();
    }
}
