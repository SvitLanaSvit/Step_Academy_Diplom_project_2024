using static System.Net.Mime.MediaTypeNames;
using System.Net;

namespace Diplom_project_2024.Data
{
    public class House
    {
        public int Id { get; set; }
        
        public string Description { get; set; } = default!;
        public double Price { get; set; }
        //public int SleepingPlaces { get; set; }
        //public int ChildrenSleepingPlaces { get; set; }
        public string Name { get; set; } = default!;
        public int AddressId { get; set; }
        public int CategoryId { get; set; }
        public int Beds { get; set; }
        public int ChildBeds {  get; set; }
        public int BabyCribs { get; set; }
        public int Pets { get; set; }
        public int Bathrooms { get; set; }
        public string AccomodationType { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public bool IsModerated { get; set; } = false;
        public User? User { get; set; }
        public Address? Address { get; set; } = default!;
        public Category? Category { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Image>? Images { get; set; }
        public List<Rent>? Rents { get; set; }
        //public List<BedInfo>? Beds { get; set;} // Жду уточнений по HouseInfo
        public List<FavoriteHouse>?  Fans { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
