namespace Diplom_project_2024.Data
{
    public class Address
    {
        public int Id { get; set; }

        public string Latitude { get; set; } = default!;
        public string Longitude { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string City { get; set; } = default!;
        public string FormattedAddress { get; set; } = default!;
        public string AddressLabel { get; set; } = default!;
        public House? House { get; set; }
    }
}
