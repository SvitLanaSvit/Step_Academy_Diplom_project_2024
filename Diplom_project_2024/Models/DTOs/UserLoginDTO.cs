namespace Diplom_project_2024.Models.DTOs
{
    public class UserLoginDTO
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool RememberMe { get; set; } 
    }
}
