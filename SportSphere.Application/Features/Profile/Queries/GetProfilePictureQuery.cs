using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;

namespace SportSphere.Application.Features.Profile.Queries
{
    public class GetProfilePictureQuery : IRequest<FileStreamResult>
    {
        public string UserId { get; set; } = null!;
    }
    public class GetProfilePictureQueryValidator : AbstractValidator<GetProfilePictureQuery>
    {
        public GetProfilePictureQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
        }
    }
    public class GetProfilePictureHandler : IRequestHandler<GetProfilePictureQuery, FileStreamResult>
    {
        private readonly IGenericRepository<Media> mediaRepo;
        private readonly IGenericRepository<MediaFolder> mediaFodlerRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public GetProfilePictureHandler(IGenericRepository<Media> mediaRepo, IGenericRepository<MediaFolder> mediaFodlerRepo, UserManager<ApplicationUser> userManager)
        {
            this.mediaRepo = mediaRepo;
            this.mediaFodlerRepo = mediaFodlerRepo;
            this.userManager = userManager;
        }

        public async Task<FileStreamResult> Handle(GetProfilePictureQuery request, CancellationToken cancellationToken)
        {
            try
            {
               Media? media = null;

                var userFolder = await mediaFodlerRepo.GetQueryable()
                    .Include(f => f.MediaFiles.Where(m => m.Category == MediaCategory.ProfilePicture))
                    .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.FolderName == "Profile Pictures", cancellationToken);

                if (userFolder?.MediaFiles?.Any() == true)
                {
                    media = userFolder.MediaFiles.OrderByDescending(m => m.CreatedAt).First();
                }
                else
                {
                    var adminFolder = await mediaFodlerRepo.GetQueryable()
                        .Include(f => f.MediaFiles.Where(m => m.Category == MediaCategory.DefaultProfileImageByAdmin))
                        .FirstOrDefaultAsync(f => f.FolderName == "Admin Pictures", cancellationToken);

                    if (adminFolder?.MediaFiles?.Any() == true)
                    {
                        media = adminFolder.MediaFiles.OrderByDescending(m => m.CreatedAt).First();
                    }
                }
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
