using System.Security.Claims;
using Illusive.Illusive.Authentication.Utility;

namespace Illusive.Illusive.Database.Models {
    public static class ForumDataExtensions {
        
        public static ForumData AssignOwner(this ForumData forumData, ClaimsPrincipal claimsPrincipal) {
            forumData.OwnerId = claimsPrincipal.GetUniqueId();
            return forumData;
        }
        
        public static bool HasLikeFrom(this ForumData forumData, ClaimsPrincipal claimsPrincipal) {
            var userId = claimsPrincipal.GetUniqueId();
            return forumData.Likes.Contains(userId);
        }
    }
}