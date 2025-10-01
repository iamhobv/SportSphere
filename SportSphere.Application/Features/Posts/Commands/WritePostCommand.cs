using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Posts.Dto;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Posts.Commands
{
    public class WritePostCommand : IRequest<ApiResponse<bool>>
    {
        public WritePostDto PostData { get; set; }
    }
    public class WritePostCommandValidator : AbstractValidator<WritePostCommand>
    {
        public WritePostCommandValidator()
        {
            RuleFor(x => x.PostData.UserId).NotEmpty().WithMessage("UserId is required");

            RuleFor(x => x.PostData.Content)
                .MaximumLength(1000).WithMessage("Content must not exceed 1000 characters");

            //RuleForEach(x => x.MediaFiles).ChildRules(media =>
            //{
            //    media.RuleFor(m => m.FileName).NotEmpty();
            //    media.RuleFor(m => m.Content).NotEmpty();
            //});
        }
    }
    public class WritePostCommandHandler : IRequestHandler<WritePostCommand, ApiResponse<bool>>
    {
        private readonly IValidator<WritePostCommand> _validator;
        private readonly IGenericRepository<Media> mediaRepo;
        private readonly IGenericRepository<Post> postRepo;
        private readonly ApplicationDbContext context;
        private readonly IGenericRepository<MediaFolder> mediaFodlerRepo;

        public WritePostCommandHandler(IValidator<WritePostCommand> validator, IGenericRepository<Media> mediaRepo, IGenericRepository<Post> postRepo, ApplicationDbContext context, IGenericRepository<MediaFolder> mediaFodlerRepo)
        {
            _validator = validator;
            this.mediaRepo = mediaRepo;
            this.postRepo = postRepo;
            this.context = context;
            this.mediaFodlerRepo = mediaFodlerRepo;
        }

        public async Task<ApiResponse<bool>> Handle(WritePostCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);
            }

            var user = await context.Users
                .Include(u => u.SocialProfile)
                .FirstOrDefaultAsync(u => u.Id == request.PostData.UserId, cancellationToken);

            if (user == null || user.SocialProfile == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "User or profile not found");

            var post = new Post
            {
                Content = request.PostData.Content ?? string.Empty,
                Caption = request.PostData.Caption,
                CreatedAt = DateTime.UtcNow,
                Author = user.SocialProfile
            };

            if (request.PostData.MediaFiles != null && request.PostData.MediaFiles.Any())
            {
                var folderid = await GetOrCreatePostsFolder(request.PostData.UserId);
                if (folderid == 0)
                {
                    return ApiResponse<bool>.Error(ErrorCode.DatabaseError, "Error while creating folder.Please try again later.");

                }


                foreach (var FileDto in request.PostData.MediaFiles)
                {
                    byte[] fileBytes;
                    using (var ms = new MemoryStream())
                    {
                        await FileDto.CopyToAsync(ms, cancellationToken);
                        fileBytes = ms.ToArray();
                    }
                    var media = new Media
                    {
                        FileName = FileDto.FileName,
                        ContentType = FileDto.ContentType,
                        Content = fileBytes,
                        MediaType = FileDto.ContentType.Contains("video") ? "Video" : "Image",
                        Category = FileDto.ContentType.Contains("video") ? MediaCategory.PostVideo : MediaCategory.PostImage,
                        CreatedAt = DateTime.UtcNow,
                        FolderId = folderid,
                        Post = post
                    };

                    await mediaRepo.AddAsync(media);
                }
            }

            await postRepo.AddAsync(post);
            await postRepo.SaveChangesAsync();



            return ApiResponse<bool>.Ok(true, "Post created successfully!");
        }
        private async Task<long> GetOrCreatePostsFolder(string userId)
        {
            var folder = await mediaFodlerRepo.GetFilteredFirstOrDefaultAsync(f => f.UserId == userId && f.FolderName == "Post Media");

            if (folder != null) return folder.Id;

            folder = new MediaFolder
            {
                UserId = userId,
                FolderName = "Post Media"
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
