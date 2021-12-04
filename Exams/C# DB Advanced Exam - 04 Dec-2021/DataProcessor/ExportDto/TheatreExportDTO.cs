using System;
using System.Collections.Generic;
using System.Text;

namespace Theatre.DataProcessor.ExportDto
{
    public class TheatreExportDTO
    {
        public string Name { get; set; }

        public int Halls { get; set; }

        public decimal TotalIncome { get; set; }

        public TicketExportDTO[] Tickets { get; set; }
    }
}
