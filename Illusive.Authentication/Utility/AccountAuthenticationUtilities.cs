using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;

namespace Illusive.Illusive.Authentication.Utility {
    public static class AccountAuthenticationUtilities {
        public static string GetUniqueId(this ClaimsPrincipal principal) {
            return principal.FindFirst(ClaimTypes.PrimarySid)?.Value;
        }
    }
}