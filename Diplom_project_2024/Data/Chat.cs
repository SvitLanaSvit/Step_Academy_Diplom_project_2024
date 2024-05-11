namespace Diplom_project_2024.Data
{
    public class Chat
    {
        public int Id { get; set; }
        public List<User>? Users { get; set; } 
        public List<Message>? Messages { get; set; }
    }
}
