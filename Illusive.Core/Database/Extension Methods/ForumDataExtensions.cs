using System.Security.Claims;
using Illusive.Utility;

namespace Illusive.Models.Extensions {
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