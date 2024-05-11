using AutoMapper;
using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;

namespace Diplom_project_2024.AutoMapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, UserDTO>();
            CreateMap<User, UserProfileInfoDTO>();
            CreateMap<Message, MessageDTO>();
            CreateMap<PaymentData,PaymentDataDTO>();
            CreateMap<House,HouseDTO>();
            CreateMap<Category,CategoryDTO>();
            CreateMap<Tag, TagDTO>();
            CreateMap<Image, ImageDTO>();
            CreateMap<Address, AddressDTO>(); 
            CreateMap<Address, AddressCreateDTO>(); 
            CreateMap<Rent,RentDTO>();
        }
    }
}   
