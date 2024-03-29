namespace Diplom_project_2024.Models.DTOs
{
    public class UserProfileInfoSetDTO
    {

        public string? firstName { get; set; } 
        public string? surname { get; set; }
        public string? gender { get; set; }
        public DateOnly? dateOfBirth { get; set; }
        public string? contactEmail { get; set; } 
        public string? phoneNumber { get; set; }
        public IFormFile? image { get; set; }
    }
}
