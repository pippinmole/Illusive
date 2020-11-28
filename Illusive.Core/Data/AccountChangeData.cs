#nullable enable
using System.ComponentModel.DataAnnotations;
using Illusive.Attributes;
using Microsoft.AspNetCore.Http;

namespace Illusive.Data {
    public class AccountChangeData {

        [StringLength(140)] public string? Bio { get; set; }

        [AllowedFormFileExtensions("jpg, jpeg, png, gif")]
        [FormFileSizeLimit(2 * 1000 * 1000)] // Max file size of 2MB
        public IFormFile? ProfilePicture { get; set; }
        
        [AllowedFormFileExtensions("jpg, jpeg, png, gif")]
        [FormFileSizeLimit(2 * 1000 * 1000)] // Max file size of 2MB
        public IFormFile? CoverPicture { get; set; }

        [StringLength(25)] public string? GithubUrl { get; set; }
        [StringLength(25)] public string? TwitterUrl { get; set; }
        [StringLength(25)] public string? RedditUrl { get; set; }
        [StringLength(25)] public string? SteamUrl { get; set; }
        [StringLength(25)] public string? LinkedInUrl { get; set; }
    }
}