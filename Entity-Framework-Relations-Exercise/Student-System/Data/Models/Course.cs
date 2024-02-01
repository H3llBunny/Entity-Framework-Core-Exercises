using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfCoreRelationsExercise.Data.Models
{
    public class Course
    {
        public Course()
        {
            this.Homeworks = new HashSet<Homework>();
            this.StudentCourses = new HashSet<StudentCourse>();
        }

        [Key]
        public int CourseId { get; set; }

        [MaxLength(80)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public ICollection<Homework> Homeworks { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
