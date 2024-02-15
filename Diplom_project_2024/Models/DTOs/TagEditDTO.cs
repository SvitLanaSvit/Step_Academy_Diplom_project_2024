namespace Diplom_project_2024.Models.DTOs
{
    public class TagEditDTO
    {
        public int Id { get; set; } 
        public string Name { get; set; } = default!;
        public IFormFile? Image { get; set; }
    }
}
