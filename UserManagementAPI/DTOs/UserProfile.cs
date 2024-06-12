using AutoMapper;
using UserManagementAPI.Models;

namespace UserManagementAPI.DTOs
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.First, opt => opt.MapFrom(src => src.First))
                .ForMember(dest => dest.Last, opt => opt.MapFrom(src => src.Last))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
