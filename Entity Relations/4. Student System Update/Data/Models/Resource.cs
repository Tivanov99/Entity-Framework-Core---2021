namespace P01_StudentSystem.Data.Models
{
    using P01_StudentSystem.Enums;
    using P01_StudentSystem.Validations;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [MaxLength(ConstantValidatons.ResourceNameMaxLength)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        [MaxLength(ConstantValidatons.UrlMaxLength)]
        [DataType("varchar")]
        public string Url { get; set; }

        public ResourceTypes ResourceType { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
