namespace Diplom_project_2024.Data
{
    public class FavoriteHouse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public int HouseId { get; set; }
        public User? User { get; set; }
        public House? House { get; set; }
    }
}
