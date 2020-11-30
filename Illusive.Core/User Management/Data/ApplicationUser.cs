using System;
using System.Security.Claims;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository.Attributes;

namespace Illusive {
    // Name this collection Users
    [CollectionName("user_info")]
    public class ApplicationUser : MongoIdentityUser<Guid> {

        public string ApiKey { get; set; }
        
        public uint Age { get; set; }
        public string ProfilePicture { get; set; }
        public string CoverPicture { get; set; }
        public string Bio { get; set; }

        public string GithubUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string RedditUrl { get; set; }
        public string SteamUrl { get; set; }
        public string LinkedinUrl { get; set; }
        public string Location { get; set; }

        public ApplicationUser() : base() {
            this.Age = 17;
            this.ProfilePicture =
                "https://t4.ftcdn.net/jpg/00/64/67/63/240_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg";
            this.CoverPicture = "https://illusivecdn.blob.core.windows.net/container-1/default-cover-photo.png";
            this.Bio = "I'm new here!";
            this.Location = "";
        }

        public ApplicationUser(string userName, string email) : base(userName, email) {
            this.Age = 17;
            this.ProfilePicture =
                "https://t4.ftcdn.net/jpg/00/64/67/63/240_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg";
            this.CoverPicture = "https://illusivecdn.blob.core.windows.net/container-1/default-cover-photo.png";
            this.Bio = "I'm new here!";
            this.Location = "";
        }

        public static readonly ApplicationUser NoUser = new ApplicationUser("Deleted User", "unknown@unknown.com");
    }

    public class ApplicationRole : MongoIdentityRole<Guid> {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}