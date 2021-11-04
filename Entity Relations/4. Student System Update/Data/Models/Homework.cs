using P01_StudentSystem.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    [Table("HomeworkSubmissions")]
    public class Homework
    {
        [Key]
        public int HomeworkId { get; set; }

        public string Content { get; set; }

        public ContentTypes ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
