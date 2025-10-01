using MagiXSquad.Domain.Interfaces;

namespace MagiXSquad.Infrastructure.Services.Implementation
{
    public class FileService : IFileService
    {
        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("Invalid file");

            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(rootPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream, cancellationToken);
            }

            return $"/{folder}/{uniqueFileName}";
        }
    }
}
