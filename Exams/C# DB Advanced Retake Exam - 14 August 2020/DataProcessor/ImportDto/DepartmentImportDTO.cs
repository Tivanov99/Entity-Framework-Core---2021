using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentImportDTO
    {
        public DepartmentImportDTO()
        {
            Cells = new HashSet<CellImportDTO>();
        }

        [Required]
        [MaxLength(25)]
        [MinLength(3)]
        public string Name { get; set; }

        [MinLength(1)]
        public ICollection<CellImportDTO> Cells { get; set; }
    }
}
