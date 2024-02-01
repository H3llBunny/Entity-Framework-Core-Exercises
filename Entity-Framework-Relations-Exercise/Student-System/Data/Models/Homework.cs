using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfCoreRelationsExercise.Data.Models
{
    public class Homework
    {
        [Key]
        public int HomeworkId { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(255)]
        public string Contnet { get; set; }

        public ContentType ContentType { get; set; }

        public TimeOnly SubmissionTime { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
