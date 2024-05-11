using Diplom_project_2024.Data;

namespace Diplom_project_2024.Models.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = default!;
        public bool IsRead { get; set; }
        public DateTime SendingTime { get; set; } = default!;
        public UserDTO FromUser { get; set; } = new UserDTO();

    }
}
