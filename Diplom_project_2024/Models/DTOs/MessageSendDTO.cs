namespace Diplom_project_2024.Models.DTOs
{
    public class MessageSendDTO
    {
        public int? ChatId {  get; set; }
        public string? ToUserId { get; set; }
        public string Content { get; set; } = default!;

    }
}
