namespace Diplom_project_2024.Data
{
    public class BedInfo
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public House House { get; set; } = new House();
        public int BedTypeId { get; set; }
        public BedType BedType { get; set; } = new BedType();
        public int Quantity { get; set; }
    }
}
