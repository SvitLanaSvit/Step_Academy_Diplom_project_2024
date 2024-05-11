namespace Diplom_project_2024.Models.DTOs
{
    public class RentCreationDTO
    {
        public int houseId { get; set; }
        public int? countOfDay { get; set; }
        public double price { get; set; }
        public string? from { get; set; }
        public string? to { get; set; }
        public PaymentDataDTO? PaymentData { get; set; }
    }   
}
