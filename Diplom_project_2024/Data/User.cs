using Microsoft.AspNetCore.Identity;

namespace Diplom_project_2024.Data
{
    public class User : IdentityUser
    {
        //public override string Id { get => base.Id; set => base.Id = value; }
        public string DisplayName { get; set; } = default!;
        public List<House>? Houses { get; set; }
        public List<Rent>? Rents { get; set; }
        public string? ImagePath { get; set; } = default!;
        public override string Email { get; set; }
    }
}
