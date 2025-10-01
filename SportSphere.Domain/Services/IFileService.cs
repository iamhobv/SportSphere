using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiXSquad.Domain.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);
    }
}
