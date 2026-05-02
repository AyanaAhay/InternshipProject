using System.ComponentModel.DataAnnotations;
using StudentApi.Contracts.Enums;

//namespace StudentApi.DTOs;
namespace StudentApi.Contracts.DTOs
{
    // Создание отзыва о практике
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

    // Обновление отзыва
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

    // Ответ с данными отзыва
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