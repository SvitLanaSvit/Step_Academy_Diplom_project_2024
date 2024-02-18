using Diplom_project_2024.Data;

namespace Diplom_project_2024.Models.DTOs
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public UserDTO ChatWith { get; set; } = new UserDTO();
        public MessageDTO LastMessage { get; set; } = new MessageDTO();
        public int CountOfUnreadMessages { get; set; }

    }
}
