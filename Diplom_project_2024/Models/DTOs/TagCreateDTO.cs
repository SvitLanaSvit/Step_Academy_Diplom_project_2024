namespace Diplom_project_2024.Models.DTOs
{
    public class TagCreateDTO
    {
        public string Name { get; set; } = default!;
        public IFormFile Image {  get; set; } = null!;
    }
}
