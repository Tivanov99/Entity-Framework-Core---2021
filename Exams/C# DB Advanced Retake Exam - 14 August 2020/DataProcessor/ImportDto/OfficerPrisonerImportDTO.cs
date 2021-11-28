using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Prisoner")]
    public class OfficerPrisonerImportDTO
    {
        [XmlAttribute("id")]
        public int Prisoner { get; set; }
    }
}
