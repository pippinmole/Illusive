#nullable enable
using System.ComponentModel.DataAnnotations;
using Illusive.Attributes;
using Microsoft.AspNetCore.Http;

namespace Illusive.Illusive.Data {
    public class AccountChangeData {
        
        [StringLength(140)]
        public string? Bio { get; set; }
        
        [AllowedFormFileExtensions("jpg, jpeg, png, gif")]
        [FormFileSizeLimit(2 * 1000 * 1000)] // Max file size of 2MB
        public IFormFile? ProfilePicture { get; set; }
    }
}