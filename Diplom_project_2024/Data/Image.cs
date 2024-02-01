namespace Diplom_project_2024.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string Path { get; set; } = default!;
        public bool IsMain { get; set; }
        public int HouseId { get; set; }
        public House House { get; set; } = default!;

    }
}
