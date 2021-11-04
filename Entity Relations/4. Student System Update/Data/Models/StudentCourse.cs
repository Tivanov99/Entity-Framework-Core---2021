namespace P01_StudentSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class StudentCourse
    {
        //TODO:Make foreign key and Composite Primary key, add naviagitons properties
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
