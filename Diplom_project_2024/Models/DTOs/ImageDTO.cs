namespace Diplom_project_2024.Models.DTOs
{
    public class ImageDTO
    {
        public int Id { get; set; }
        public string Path { get; set; } = default!;
        public bool IsMain { get; set; }
    }
}
