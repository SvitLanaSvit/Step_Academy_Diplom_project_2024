﻿namespace Diplom_project_2024.Models.DTOs
{
    public class UserProfileInfoDTO
    {
        public string firstName { get; set; } = default!;
        public string surname { get; set; } = default!;
        public string gender { get; set; } = default!;
        public DateOnly? dateOfBirth { get; set; }
        public string contactEmail { get; set; } = default!;
        public string phoneNumber { get; set; } = default!;

        public string cardNumber { get; set; } = default!;
        public DateOnly? expireDate { get; set; } = default!;
        public string cvv { get; set; } = default!;
        public int countOfHouses { get; set; }
        public int countOfComments {  get; set; }
        public string imagePath {  get; set; } = default!;

    }
}
