using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom_project_2024.Data
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public User User { get; set; } = new User();    
        [Column(name: "RefreshToken")]
        public string refreshToken { get; set; } = default!;
        public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow;

    }
}
