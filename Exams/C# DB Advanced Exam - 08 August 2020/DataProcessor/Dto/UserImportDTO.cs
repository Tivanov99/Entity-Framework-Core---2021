using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.DataProcessor.Dto
{
    public class UserImportDTO
    {
        [RegularExpression(@"^[A-Z][a-z]*\s[A-Z][a-z]*\s")]
        public string FullName { get; set; }

        [MaxLength(20)]
        [MinLength(3)]
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Range(3, 103)]
        public int Age { get; set; }

        public CardImportDTO[] Cards { get; set; }
    }
}
