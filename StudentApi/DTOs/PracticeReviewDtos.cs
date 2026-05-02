using System.ComponentModel.DataAnnotations;

namespace StudentApi.DTOs
{
    /// <summary>
    /// Создание отзыва о практике
    /// </summary>
    public class CreatePracticeReviewDto
    {
        [Required]
        public int IdStudentApplication { get; set; }
        public string? Comment { get; set; }
        public bool ReadyToWork { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int SpecialityRelevance { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int SupervisionQuality { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int ExperienceUsefulness { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int OverallScore { get; set; }
    }

    /// <summary>
    /// Обновление отзыва
    /// </summary>
    public class UpdatePracticeReviewDto
    {
        public string? Comment { get; set; }
        public bool ReadyToWork { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int SpecialityRelevance { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int SupervisionQuality { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int ExperienceUsefulness { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
        public int OverallScore { get; set; }
    }

    /// <summary>
    /// Ответ с данными отзыва
    /// </summary>
    public class PracticeReviewResponseDto
    {
        public int IdPracticeReview { get; set; }
        public int IdStudentApplication { get; set; }
        public string? StudentName { get; set; }
        public string? Comment { get; set; }
        public bool ReadyToWork { get; set; }
        public int SpecialityRelevance { get; set; }
        public int SupervisionQuality { get; set; }
        public int ExperienceUsefulness { get; set; }
        public int OverallScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}