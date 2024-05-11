namespace Diplom_project_2024.Models.DTOs
{
    public class UserBasicInfoDTO
    {
        public string firstname { get; set; } = default!;
        public string surname { get; set; } = default!;
        public string gender { get; set; } = default!;
        public DateOnly? dateOfBirth { get; set; }

    }
}
