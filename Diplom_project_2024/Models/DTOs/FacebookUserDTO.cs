namespace Diplom_project_2024.Models.DTOs
{
    public class FacebookUserDTO
    {
        public string? firstName { get; set; } 
        public string? surName { get; set; } 
        public string email { get; set; } = default!;
        public string? picture { get; set; }

    }
}
