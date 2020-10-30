using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Cdn.Interfaces;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Illusive.Pages {
    public class AccountModel : PageModel {

        private readonly ILogger<AccountModel> _logger;
        private readonly IAccountService _accountService;
        private readonly IContentService _contentService;
        
        private static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
        private static readonly long MaxFileSizeBytes = 5 * 1000 * 1000;
        
        public IFormFile FileUpload { get; set; }

        public AccountModel(ILogger<AccountModel> logger, IAccountService accService, IContentService contentService) {
            this._logger = logger;
            this._accountService = accService ?? throw new NullReferenceException("AccountService needs to be setup!");
            this._contentService = contentService;
        }

        public IActionResult OnGet() {
            if ( !this.User.Identity.IsAuthenticated )
                return this.RedirectToPage("/Index");

            return this.Page();
        }
        
        public async Task<IActionResult> OnPostUploadAsync() {
            var accountId = this.User.FindFirst(ClaimTypes.PrimarySid)?.Value;
            
            var file = this.FileUpload;
            if ( file != null && file.Length > 0 ) {
                if ( !ImageExtensions.Contains(Path.GetExtension(file.FileName)?.ToUpperInvariant()) ) {
                    this.ModelState.AddModelError("",
                        $"Wrong file-type provided! Please use the file types {ImageExtensions.Aggregate((a, b) => $"{a}, {b}")}");
                    return this.Page();
                }
                if ( file.Length > MaxFileSizeBytes ) {
                    this.ModelState.AddModelError("",
                        $"The file provided exceeds the maximum size of {MaxFileSizeBytes / 1000 / 1000}MB!");
                    return this.Page();
                }
                
                var filePath = Path.GetTempFileName();

                await using var stream = System.IO.File.Create(filePath, (int) file.Length);
                await file.CopyToAsync(stream);

                var path = await this._contentService.UploadFileAsync(Path.GetFileName(file.FileName), stream);
                
                var update = Builders<AccountData>.Update.Set(x => x.ProfilePicture, path);
                this._accountService.UpdateAccount(accountId, update);
            }
            
            return this.Page();
        }
    }
}