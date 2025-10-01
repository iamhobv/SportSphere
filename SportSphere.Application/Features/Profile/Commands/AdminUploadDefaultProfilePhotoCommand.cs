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

namespace SportSphere.Application.Features.Profile.Commands
{

    public class AdminUploadDefaultProfilePhotoCommand : IRequest<ApiResponse<long>>
    {
        public string UserId { get; set; } = null!;
        public IFormFile File { get; set; } = null!;
    }
    public class AdminUploadDefaultProfilePhotoCommandValidator : AbstractValidator<AdminUploadDefaultProfilePhotoCommand>
    {
        public AdminUploadDefaultProfilePhotoCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required")
                .Must(f => f!.Length > 0).WithMessage("File cannot be empty")
                .Must(f => f!.ContentType.StartsWith("image")).WithMessage("Only image files are allowed");
        }
    }
    public class AdminUploadDefaultProfilePhotoCommandHandler : IRequestHandler<AdminUploadDefaultProfilePhotoCommand, ApiResponse<long>>
    {
        private readonly IGenericRepository<Media> mediaRepo;
        private readonly IGenericRepository<MediaFolder> mediaFodlerRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminUploadDefaultProfilePhotoCommandHandler(IGenericRepository<Media> mediaRepo, IGenericRepository<MediaFolder> mediaFodlerRepo, UserManager<ApplicationUser> userManager)
        {
            this.mediaRepo = mediaRepo;
            this.mediaFodlerRepo = mediaFodlerRepo;
            this.userManager = userManager;
        }
        public async Task<ApiResponse<long>> Handle(AdminUploadDefaultProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await request.File.CopyToAsync(ms, cancellationToken);
                fileBytes = ms.ToArray();
            }

            // Save media in database
            var folderid = await GetOrCreateProfileFolder(request.UserId);
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
                Category = MediaCategory.DefaultProfileImageByAdmin

            };

            await mediaRepo.AddAsync(media);
            await mediaRepo.SaveChangesAsync();

            // Update user's profile image reference if needed
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user != null)
            {
                user.ProfileImage = media.Id.ToString();
                var updateUserResult = await userManager.UpdateAsync(user);
                if (updateUserResult.Succeeded)
                {
                    return ApiResponse<long>.Ok(media.Id);

                }
                return ApiResponse<long>.Error(ErrorCode.DatabaseError, "Error while uploading image.Please try again later.");


            }

            return ApiResponse<long>.Ok(media.Id);
        }
        private async Task<long> GetOrCreateProfileFolder(string userId)
        {
            var folder = await mediaFodlerRepo.GetFilteredFirstOrDefaultAsync(f => f.UserId == userId && f.FolderName == "Admin Pictures");

            if (folder != null) return folder.Id;

            folder = new MediaFolder
            {
                UserId = userId,
                FolderName = "Admin Pictures"
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
