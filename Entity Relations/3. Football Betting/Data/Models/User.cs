﻿namespace P03_FootballBetting.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class User
    {
        public User()
        {
            Bets = new HashSet<Bet>();
        }
        [Key]
        public int UserId { get; set; }

        [DataType("nvarchar(30)")]
        public string Username { get; set; }

        [DataType("nvarchar(30)")]
        public string Password { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public ICollection<Bet> Bets { get; set; }
    }
}