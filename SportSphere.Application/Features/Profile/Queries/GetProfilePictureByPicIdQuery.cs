using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.Profile.Queries
{

    public class GetProfilePictureByPicIdQuery : IRequest<FileStreamResult>
    {
        public long PictureId { get; set; }
    }
    public class GetProfilePictureByPicIdQueryValidator : AbstractValidator<GetProfilePictureByPicIdQuery>
    {
        public GetProfilePictureByPicIdQueryValidator()
        {
            RuleFor(x => x.PictureId)
                .NotEmpty().WithMessage("PictureId is required");
        }
    }

    public class GetProfilePictureByPicIdQueryHandler : IRequestHandler<GetProfilePictureByPicIdQuery, FileStreamResult>
    {
        private readonly IGenericRepository<Media> mediaRepo;
        private readonly IGenericRepository<MediaFolder> mediaFodlerRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public GetProfilePictureByPicIdQueryHandler(IGenericRepository<Media> mediaRepo, IGenericRepository<MediaFolder> mediaFodlerRepo, UserManager<ApplicationUser> userManager)
        {
            this.mediaRepo = mediaRepo;
            this.mediaFodlerRepo = mediaFodlerRepo;
            this.userManager = userManager;
        }

        public async Task<FileStreamResult> Handle(GetProfilePictureByPicIdQuery request, CancellationToken cancellationToken)
        {
            //var folder = await mediaFodlerRepo.GetQueryable()
            //    .Include(f => f.MediaFiles)
            //    .Where(f => f.MediaFiles. == request.PictureId && f.FolderName == "Profile Pictures", cancellationToken);





            try
            {
                Media? media = null;

                media =await mediaRepo.GetFilteredFirstOrDefaultAsync(f => f.Id == request.PictureId);

              
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
