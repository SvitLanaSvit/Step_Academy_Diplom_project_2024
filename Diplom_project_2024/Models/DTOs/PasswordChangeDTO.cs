namespace Diplom_project_2024.Models.DTOs
{
    public class PasswordChangeDTO
    {
        public string password { get; set; } = default!;
        public string oldPassword { get; set;} = default!;
    }
}
