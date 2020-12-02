using System;

namespace Illusive {
    public class SafeApplicationUser {
        public Guid Id { get; set; }
        public string UserName { get; set; }
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
    }
}