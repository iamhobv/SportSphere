using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.Repos;

namespace SportSphere.Application.Features.Profile.Commands
{
    public class UploadProfilePictureCommand : IRequest<ApiResponse<long>>
    {
        public IFormFile File { get; set; } = null!;
        public string? UserId { get; set; }
    }
    public class UploadProfilePictureCommandValidator : AbstractValidator<UploadProfilePictureCommand>
    {
        public UploadProfilePictureCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required")
                .Must(f => f!.Length > 0).WithMessage("File cannot be empty")
                .Must(f => f!.ContentType.StartsWith("image")).WithMessage("Only image files are allowed");
        }
    }


    public class UploadProfilePictureHandler : IRequestHandler<UploadProfilePictureCommand, ApiResponse<long>>
    {
        private readonly IGenericRepository<Media> mediaRepo;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGenericRepository<MediaFolder> mediaFodlerRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public UploadProfilePictureHandler(IGenericRepository<Media> mediaRepo, IHttpContextAccessor httpContextAccessor, IGenericRepository<MediaFolder> mediaFodlerRepo, UserManager<ApplicationUser> userManager)
        {
            this.mediaRepo = mediaRepo;
            this.httpContextAccessor = httpContextAccessor;
            this.mediaFodlerRepo = mediaFodlerRepo;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<long>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
        {
            // Convert file to byte array
            string? currentUserId = null;
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await request.File.CopyToAsync(ms, cancellationToken);
                fileBytes = ms.ToArray();
            }
            if (request.UserId != null && request.UserId.Length > 0)
            {
                currentUserId = request.UserId;
            }
            else
            {

                currentUserId = httpContextAccessor.HttpContext?.User?
    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return ApiResponse<long>.Error(ErrorCode.Unauthorized, "You must login first");
                }
            }

            // Save media in database
            var folderid = await GetOrCreateProfileFolder(currentUserId);
            if (folderid == 0)
            {
                return ApiResponse<long>.Error(ErrorCode.DatabaseError, "Error while creating folder.Please try again later.");

            }
            var media = new Media
            {
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                Content = fileBytes,
                MediaType = "image",
                FolderId = folderid,
                Category = MediaCategory.ProfilePicture
            };

            await mediaRepo.AddAsync(media);
            await mediaRepo.SaveChangesAsync();

            // Update user's profile image reference if needed
            var user = await userManager.FindByIdAsync(currentUserId);
            if (user != null)
            {
                user.ProfileImage = media.Id.ToString();
                var updateUserResult = await userManager.UpdateAsync(user);
                if (updateUserResult.Succeeded)
                {
                    return ApiResponse<long>.Ok(media.Id);

                }
                return ApiResponse<long>.Error(ErrorCode.DatabaseError, "Error while Updating Profile Picture.Please try again later.");


            }

            return ApiResponse<long>.Ok(media.Id);
        }

        // helper: get or create a folder named "Profile Pictures" for user
        private async Task<long> GetOrCreateProfileFolder(string userId)
        {
            var folder = await mediaFodlerRepo.GetFilteredFirstOrDefaultAsync(f => f.UserId == userId && f.FolderName == "Profile Pictures");
            //.FirstOrDefault(f => f.UserId == userId && f.FolderName == "Profile Pictures");

            if (folder != null) return folder.Id;

            folder = new MediaFolder
            {
                UserId = userId,
                FolderName = "Profile Pictures"
            };

            await mediaFodlerRepo.AddAsync(folder);
            var saveRes = await mediaFodlerRepo.SaveChangesAsync();
            if (saveRes)
            {

                return folder.Id;
            }
            else
            {
                return 0;
            }
        }
    }
}
