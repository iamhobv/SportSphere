using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;
using SportSphere.Domain.Enums;

namespace SportSphere.Domain.Entities
{
    [Index(nameof(FileName), IsUnique = false)]

    public class Media : BaseEntity
    {
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public byte[] Content { get; set; } = null!;
        public MediaCategory Category { get; set; } = MediaCategory.Other;
        public string MediaType { get; set; } = null!; // "image" or "video"

        public long FolderId { get; set; }       // FK to MediaFolder
        public MediaFolder Folder { get; set; } = null!;

        public long? PostId { get; set; }       // optional FK to Post
        public Post? Post { get; set; }
    }
}
