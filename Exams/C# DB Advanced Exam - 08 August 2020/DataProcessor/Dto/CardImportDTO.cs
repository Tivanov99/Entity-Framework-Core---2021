using System;
using System.Collections.Generic;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto
{
    public class CardImportDTO
    {
        public string Number { get; set; }

        public string CVC { get; set; }

        public string Type { get; set; }
    }
}
