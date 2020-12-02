using AutoMapper;

namespace Illusive.Illusive.Core.User_Management.Maps {
    public class AppUserMapProfile : Profile {
        public AppUserMapProfile() {
            this.CreateMap<ApplicationUser, SafeApplicationUser>();
        }
    }
}