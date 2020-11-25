﻿using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Illusive {
    // Name this collection Users
    [CollectionName("user_info")]
    public class ApplicationUser : MongoIdentityUser<Guid> {

        public uint Age { get; set; }
        public string ProfilePicture { get; set; }
        public string Bio { get; set; }
        public bool IsAdmin { get; set; }

        public ApplicationUser() : base() {
            this.Age = 17;
            this.ProfilePicture = "https://t4.ftcdn.net/jpg/00/64/67/63/240_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg";
            this.Bio = "I'm new here!";
            this.IsAdmin = false;
        }

        public ApplicationUser(string userName, string email) : base(userName, email) {
            this.Age = 17;
            this.ProfilePicture = "https://t4.ftcdn.net/jpg/00/64/67/63/240_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg";
            this.Bio = "I'm new here!";
            this.IsAdmin = false;
        }
    }

    public class ApplicationRole : MongoIdentityRole<Guid> {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}