using System;
using System.Collections.Generic;
using System.Text;

namespace SoftJail.DataProcessor.ExportDto
{
    public class PrisonerExportDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CellNumber { get; set; }

        public PrisonerOfficerExportDTO[] Officers { get; set; }

        public decimal TotalOfficerSalary { get; set; }

    }
}
