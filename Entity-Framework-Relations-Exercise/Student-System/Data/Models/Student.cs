using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfCoreRelationsExercise.Data.Models
{
    public class Student
    {
        public Student()
        {
            this.Homerworks = new HashSet<Homework>();
            this.StudentCourse = new HashSet<StudentCourse>();
        }

        [Key]
        public int StudentId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [MinLength(10)]
        public string? PhoneNumber { get; set; }

        public DateOnly RegisteredOn { get; set; }

        public DateOnly? Birthday { get; set; }

        public ICollection<Homework> Homerworks { get; set; }

        public ICollection<StudentCourse> StudentCourse { get; set; }
    }
}
