using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Application.Features.Profile.DTOs
{
    public class UserProfileVm
    {
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public GenderType Gender { get; set; }
        public UserRoles Role { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Sport { get; set; }
        public string? Psition { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }

        public string UserId { get; set; } = null!;
        public string? ProfileImage { get; set; }


    }

    public class PostVm
    {
        public long PostId { get; set; }
        public string Content { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public string? Caption { get; set; }
        //public ICollection<FileStreamResult> MediaFiles { get; set; } = new List<FileStreamResult>();
        public ICollection<MediaVm> MediaFiles { get; set; } = new List<MediaVm>();
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public bool? IsILiked { get; set; }
        public string UserId { get; set; } = null!;
        public bool isFollowed { get; set; }


        public DateTime CreatedAt { get; set; }
    }

    public class MediaVm
    {
        public long MediaId { get; set; }
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        public string Url { get; set; } = null!;
    }
    public class MediaFolderVm
    {
        public long FolderId { get; set; }
        public string FolderName { get; set; } = null!;
        public ICollection<MediaVm> MediaFiles { get; set; } = new List<MediaVm>();
    }


    public class AthleteAchievementVm
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
    }
}
