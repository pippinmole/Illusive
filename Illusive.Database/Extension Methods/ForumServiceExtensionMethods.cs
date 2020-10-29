using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Extension_Methods {
    public static class ForumServiceExtensionMethods {
        
        public static ForumData GetForumById(this IForumService service, string id) {
            return service.GetForumWhere(x => x.Id == id);
        }
    }
}