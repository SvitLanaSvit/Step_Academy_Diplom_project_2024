namespace Diplom_project_2024.Data
{
    public class Message
    {
        public int Id { get; set; }
        public string FromUserId { get; set; } = default!;
        public User FromUser { get; set; } = new User();
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = new Chat();
        public string Content = default!;
        public DateTime SendingTime { get; set; } = new DateTime();
        public bool IsRead { get; set; } 
    }
}
