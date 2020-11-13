using System.Security.Claims;
using Illusive.Models;

namespace Illusive.Utility {
    public static class ClaimsIdentityUtilities {
        public static void AddClaimsForAccount(this ClaimsIdentity identity, AccountData account) {
            identity.AddClaim(new Claim(ClaimTypes.Name, account.AccountName));
            identity.AddClaim(new Claim(ClaimTypes.Email, account.Email));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, account.Id));
            identity.AddClaim(new Claim(ClaimTypes.Role, account.IsAdmin ? "Admin" : "Standard"));
        }
    }
}