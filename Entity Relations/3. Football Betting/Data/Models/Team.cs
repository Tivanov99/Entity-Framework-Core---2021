using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    [Table("Teams")]
    public class Team
    {
        public Team()
        {
            HomeGames = new HashSet<Game>();

            AwayGames = new HashSet<Game>();

            Players = new HashSet<Player>();
        }

        [Key]
        public int TeamId { get; set; }

        [MaxLength(60)]
        public string Name { get; set; }

        //TODO: Make Unicode
        public string LogoUrl { get; set; }


        //Make Unicode
        [DataType("varchar(4)")]
        public string Initials { get; set; }

        public Decimal Budget { get; set; }

        [ForeignKey(nameof(PrimaryKitColor))]
        public int PrimaryKitColorId { get; set; }
        public virtual Color PrimaryKitColor { get; set; }

        [ForeignKey(nameof(SecondaryKitColor))]
        public int SecondaryKitColorId { get; set; }
        public virtual Color SecondaryKitColor { get; set; }

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }
        public Town Town { get; set; }


        [InverseProperty("HomeTeam")]
        public virtual ICollection<Game> HomeGames { get; set; }

        [InverseProperty("AwayTeam")]
        public virtual ICollection<Game> AwayGames { get; set; }

        public Country Country { get; set; }

        public virtual ICollection<Player> Players { get; set; }

    }
}
