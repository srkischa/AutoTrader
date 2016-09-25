using AutoMapper;
using AutoTrader.Service.Extensions;
using AutoTrader.Service.Identity;
using AutoTrader.WebApi.Request;

namespace AutoTrader.WebApi.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegistrationRequest, ApplicationUser>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.PasswordHash)
                .Ignore(dest => dest.SecurityStamp)
                .Ignore(dest => dest.Roles)
                .Ignore(dest => dest.EmailConfirmed);
        }
    }
}