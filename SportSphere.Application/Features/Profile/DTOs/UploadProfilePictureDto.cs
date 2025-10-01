using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SportSphere.Application.Features.Profile.DTOs
{
    public class UploadProfilePictureDto
    {
        public IFormFile File { get; set; } = null!;
    }

}
