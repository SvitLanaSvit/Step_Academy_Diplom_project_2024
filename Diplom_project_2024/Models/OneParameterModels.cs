namespace Diplom_project_2024.Models
{
    public class SetFirstNameModel
    {
        public string firstName { get; set; } = default!;
    }
    public class SetSurnameModel
    {
        public string surname { get; set; } = default!;
    }
    public class SetGenderModel
    {
        public string gender { get; set; } = default!;
    }
    public class SetDateOfBirthModel
    {
        public DateOnly? dateOfBirth { get; set; }
    }
    public class SetPhoneNumber
    {
        public string phoneNumber { get; set; } = default!;
    }
    public class SetContactEmail
    {
        public string contactEmail { get; set; } = default!;
    }
    public class FavoriteHouseIdDTO
    {
        public int houseId { get; set; }
    }
}
