using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Album
    {

        public Album()
        {
            Songs = new HashSet<Song>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        //TODO: the sum of all song prices in the album 
        public Decimal Price => CalculateSongsPrices();

        [ForeignKey(nameof(Producer))]
        public int? ProducerId { get; set; }
        public Producer Producer { get; set; }

        public ICollection<Song> Songs { get; set; }

        private decimal CalculateSongsPrices()
        {
            decimal sum = 0;
            foreach (var item in Songs)
            {
                sum += item.Price;
            }
            return sum;
        }
    }
}
