using Microsoft.AspNetCore.Identity;

namespace Diplom_project_2024.Data
{
    public class User : IdentityUser
    {
        //public override string Id { get => base.Id; set => base.Id = value; }
        //public string DisplayName { get; set; } = default!;
        public string FirstName { get; set; } = "";
        public string Surname { get; set; } = "";
        public string Gender { get; set; } = "";
        public DateOnly? DateOfBirth { get; set; } = null;
        public string ContactEmail {get;set; } = string.Empty;
        public override string PhoneNumber {  get; set; } = string.Empty;
        public string? ImagePath { get; set; } = default!;
        public override string Email { get; set; } = default!;
        public List<House>? Houses { get; set; }
        public List<Rent>? Rents { get; set; }
        
        public List<Chat>? Chats { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
        public PaymentData? PaymentData { get; set; }
        public List<FavoriteHouse>? FavoriteHouses { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
