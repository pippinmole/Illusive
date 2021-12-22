#nullable enable
using System;
using System.Threading.Tasks;
using Illusive.Data;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class AccountModel : PageModel {

        private readonly ILogger<AccountModel> _logger;
        private readonly IAppUserManager _userManager;
        private readonly IContentService _contentService;
        
        [BindProperty] public AccountChangeData AccountUpdate { get; set; }
        
        public AccountModel(ILogger<AccountModel> logger, IAppUserManager userManager, IContentService contentService) {
            _logger = logger;
            _userManager = userManager ?? throw new NullReferenceException("AccountService needs to be setup!");
            _contentService = contentService;
        }

        public async Task<IActionResult> OnGetAsync(string? id) {
            if ( id == null || !Guid.TryParse(id, out var guid) ) {
                _logger.LogInformation($"User attempted to access invalid user id - redirecting...");
                
                if ( User.IsLoggedIn() )
                    return Redirect($"/Account/{User.GetUniqueId()}");

                return LocalRedirect("/");
            }

            var account = await _userManager.GetUserByIdAsync(guid);
            if ( account == null ) return Redirect("/");

            return Page();
        }

        public class DeleteAccountPost {
            public Guid accountId;
        }
        
        public async Task<IActionResult> OnPostDeleteAccountAsync([FromBody] DeleteAccountPost post) {
            if ( !ModelState.IsValid )
                return Page();

            var reqAccountId = post.accountId;
            if ( User.GetUniqueId() != reqAccountId ) {
                _logger.LogWarning($"{User.GetUniqueId()} tried to delete account of id {reqAccountId}!");
                return Forbid();
            }

            var user = await _userManager.GetUserByIdAsync(reqAccountId);
            var result = await _userManager.RemoveUserAsync(user);
            
            if ( result.Succeeded ) {
                _logger.LogInformation($"{user.UserName} has been deleted");

                await _userManager.SignOutAsync();
            } else {
                _logger.LogError($"Error when trying to delete user {user.UserName}.");
            }

            return new JsonResult(new {
                Redirect = "/Index"
            });
        }
    }
}