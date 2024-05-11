namespace Diplom_project_2024.Data
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public User? User { get; set; }
        public int HouseId { get; set; }
        public House? House { get; set; }
        public string Content { get; set; } = default!;
        public int Rating { get; set; } = default!;
    }
}
