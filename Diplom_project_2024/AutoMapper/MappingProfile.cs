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
            CreateMap<Message, MessageDTO>();
        }
    }
}
