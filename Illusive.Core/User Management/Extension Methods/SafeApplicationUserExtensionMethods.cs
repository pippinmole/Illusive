using System.Security.Claims;
using Illusive.Utility;

namespace Illusive.Illusive.Core.User_Management.Extension_Methods {
    public static class SafeApplicationUserExtensionMethods {

        public static bool IsFollowing(this SafeApplicationUser user, SafeApplicationUser target) {
            return user.Following.Contains(target.Id.ToString());
        }
        public static bool IsFollowing(this SafeApplicationUser user, ApplicationUser target) {
            return user.Following.Contains(target.Id.ToString());
        }
        public static bool HasFollower(this SafeApplicationUser user, SafeApplicationUser target) {
            return user.Followers.Contains(target.Id.ToString());
        }
        public static bool HasFollower(this SafeApplicationUser user, ApplicationUser target) {
            return user.Followers.Contains(target.Id.ToString());
        }
        
        public static bool IsFollowing(this SafeApplicationUser user, ClaimsPrincipal target) {
            return user.Following.Contains(target.GetUniqueId().ToString());
        }
        public static bool HasFollower(this SafeApplicationUser user, ClaimsPrincipal target) {
            return user.Followers.Contains(target.GetUniqueId().ToString());
        }
    }
}