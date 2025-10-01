using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSphere.Application.Features.Profile.Queries;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.MediaFeatures.Queries
{
    public class MediaPreviewQuery : IRequest<FileStreamResult>
    {
        public long PictureId { get; set; }
    }
    public class MediaPreviewQueryValidator : AbstractValidator<MediaPreviewQuery>
    {
        public MediaPreviewQueryValidator()
        {
            RuleFor(x => x.PictureId)
                .NotEmpty().WithMessage("PictureId is required");
        }
    }

    public class MediaPreviewQueryHandler : IRequestHandler<MediaPreviewQuery, FileStreamResult>
    {
        private readonly IGenericRepository<Domain.Entities.Media> mediaRepo;


        public MediaPreviewQueryHandler(IGenericRepository<Domain.Entities.Media> mediaRepo)
        {
            this.mediaRepo = mediaRepo;

        }

        public async Task<FileStreamResult> Handle(MediaPreviewQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Domain.Entities.Media? media = null;

                media = await mediaRepo.GetFilteredFirstOrDefaultAsync(f => f.Id == request.PictureId);


                if (media == null)
                {
                    var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default-profile.png");
                    if (!File.Exists(defaultImagePath))
                    {
                        var emptyBytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
                        var emptyStream = new MemoryStream(emptyBytes);
                        return new FileStreamResult(emptyStream, "image/png")
                        {
                            FileDownloadName = "empty.png"
                        };
                    }

                    var defaultBytes = await File.ReadAllBytesAsync(defaultImagePath, cancellationToken);
                    var defaultStream = new MemoryStream(defaultBytes);
                    return new FileStreamResult(defaultStream, "image/png")
                    {
                        FileDownloadName = "default-profile.png"
                    };
                }

                var stream = new MemoryStream(media.Content);
                return new FileStreamResult(stream, media.ContentType)
                {
                    FileDownloadName = media.FileName
                };
            }
            catch (Exception)
            {

                throw new FileNotFoundException("Default profile image not found.");
            }
        }
    }


}
