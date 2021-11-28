using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto
{
    [XmlType("Purchase")]
    public class PurchaseImportDTO
    {
        [XmlAttribute("title")]
        [Required]
        public string Title { get; set; }

        [Required]
        [XmlElement("Type")]
        public string Type { get; set; }

        [Required]
        [XmlElement("Key")]
        [RegularExpression(@"^[A-Z0-9]{4}[-][A-Z0-9]{4}[-][A-Z0-9]{4}$")]
        public string Key { get; set; }

        [Required]
        [XmlElement("Card")]
        [RegularExpression(@"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$")]
        public string Card { get; set; }

        [Required]
        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
