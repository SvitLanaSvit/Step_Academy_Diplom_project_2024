﻿namespace Diplom_project_2024.Data
{
    public class Rent
    {
        public int Id { get; set; }
        public string? UserId { get; set; } = default!;
        public int HouseId { get; set; }
        public int CountOfDay { get; set; }
        public double? Price { get; set; }
        public string? From { get; set; } 
        public string? To { get; set; }
        public User? User { get; set; }
        public House? House { get; set; }

    }
}
