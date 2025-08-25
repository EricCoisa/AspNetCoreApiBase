using AutoMapper;
using CoreDomainBase.Entities;
using CoreApiBase.Application.DTOs;

namespace CoreApiBase.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
