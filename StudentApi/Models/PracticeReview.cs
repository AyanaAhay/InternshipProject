using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models
{
    [Table("PracticeReview")]
    public class PracticeReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPracticeReview { get; set; }

        [Required]
        public int IdStudentApplication { get; set; }

        [ForeignKey(nameof(IdStudentApplication))]
        public virtual StudentApplication? StudentApplication { get; set; }

        [Column(TypeName = "text")]
        public string? Comment { get; set; }

        public bool ReadyToWork { get; set; } = false;

        [Required]
        [Range(1, 5)]
        public int SpecialityRelevance { get; set; } // Соответствие специальности

        [Required]
        [Range(1, 5)]
        public int SupervisionQuality { get; set; } // Качество руководства

        [Required]
        [Range(1, 5)]
        public int ExperienceUsefulness { get; set; } // Полезность опыта

        [Required]
        [Range(1, 5)]
        public int OverallScore { get; set; } // Общая оценка

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}