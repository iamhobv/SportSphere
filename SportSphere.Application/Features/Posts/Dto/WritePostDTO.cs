using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Posts.Dto
{
    public class WritePostDto
    {
        public string UserId { get; set; } = null!;
        public string? Content { get; set; }
        public string? Caption { get; set; }

        public List<IFormFile>? MediaFiles { get; set; } = new();
    }
}
