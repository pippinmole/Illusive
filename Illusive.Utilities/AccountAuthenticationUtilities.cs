using System.Security.Claims;
using Illusive.Models;

namespace Illusive.Utility {
    public static class AccountAuthenticationUtilities {

        public static string GetUniqueId(this ClaimsPrincipal principal) {
            return principal.FindFirst(ClaimTypes.PrimarySid)?.Value;
        }
        
        public static string GetDisplayName(this ClaimsPrincipal principal) {
            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static bool IsLoggedIn(this ClaimsPrincipal principal) {
            return principal.Identity.IsAuthenticated;
        }
        
        public static bool CanDeletePost(this ClaimsPrincipal principal, ForumData post) {
            return post.OwnerId == principal.GetUniqueId();
        }
    }
}