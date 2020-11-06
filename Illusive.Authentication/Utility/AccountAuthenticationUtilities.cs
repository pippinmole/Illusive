using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Authentication.Utility {
    public static class AccountAuthenticationUtilities {

        public static string GetUniqueId(this ClaimsPrincipal principal) {
            return principal.FindFirst(ClaimTypes.PrimarySid)?.Value;
        }

        public static bool IsLoggedIn(this ClaimsPrincipal principal) {
            return principal.Identity.IsAuthenticated;
        }
    }
}