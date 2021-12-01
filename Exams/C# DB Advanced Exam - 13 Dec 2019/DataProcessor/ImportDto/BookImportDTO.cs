using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BookShop.DataProcessor.ImportDto
{
    [XmlType("Book")]
    public class BookImportDTO
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [XmlElement("Genre")]
        [Range(1, 3)]
        public int Genre { get; set; }

        [XmlElement("Price")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Range(50, 5000)]
        [XmlElement("Pages")]
        public int Pages { get; set; }

        [Required]
        [XmlElement("PublishedOn")]
        public string PublishedOn { get; set; }
    }
}
