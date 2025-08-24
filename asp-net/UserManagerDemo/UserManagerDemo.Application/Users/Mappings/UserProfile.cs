using AutoMapper;
using UserManagerDemo.Application.Users.Dtos;
using UserManagerDemo.Domain.Entity;

namespace UserManagerDemo.Application.Users.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<Domain.Entity.ApplicationUserProfile, UserDto>().ReverseMap();
        CreateMap<Domain.Entity.ApplicationUserProfile, ReadUserDto>().ReverseMap();
    }
}