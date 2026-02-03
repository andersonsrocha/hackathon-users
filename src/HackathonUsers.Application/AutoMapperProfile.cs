using AutoMapper;
using HackathonUsers.Domain.Dto;
using HackathonUsers.Domain.Models;

namespace HackathonUsers.Application;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();
    }
}