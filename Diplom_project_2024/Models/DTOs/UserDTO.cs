﻿namespace Diplom_project_2024.Models.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? ImagePath { get; set; }
        public int? countOfHouses { get; set; }
        public int? countOfComments { get; set; }
    }
}
