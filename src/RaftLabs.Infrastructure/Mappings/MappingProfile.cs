using AutoMapper;
using RaftLabs.Application.DTOs.Users;
using RaftLabs.Domain.Models;

namespace RaftLabs.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
        }
    }
}
