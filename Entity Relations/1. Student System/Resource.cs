namespace P01_StudentSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Resources")]
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [Column(TypeName = "nvarcar(50)")]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public ResourceType ResourceType { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
    }
}
