using AutoMapper;
using EventEae1._2_Backend.Models;
using EventEae1._2_Backend.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventEae1._2_Backend.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterUserDto, User>(); // when registering
            CreateMap<User, UserDto>();          // when returning user info
        }
    }
}
