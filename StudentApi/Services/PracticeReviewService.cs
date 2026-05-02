using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Contracts.DTOs;
using StudentApi.Models;
//using StudentApi.DTOs;
using StudentApi.Contracts.Enums;

namespace StudentApi.Services
{
    public class PracticeReviewService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PracticeReviewService> _logger;

        public PracticeReviewService(AppDbContext context, ILogger<PracticeReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ========== Создание отзыва ==========
        public async Task<PracticeReviewResponseDto?> CreateAsync(CreatePracticeReviewDto dto)
        {
            // Проверяем, что заявка существует
            var application = await _context.StudentApplications
                .AsNoTracking()
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.IdStudentApplication == dto.IdStudentApplication);
            if (application == null) return null;

            // Отзыв можно оставить только по завершённой практике (статус Accepted)
            if (application.StudentApplicationStatus != StudentApplicationStatus.Accepted)
            {
                _logger.LogWarning("Cannot create review for application {Id} — status is {Status}", dto.IdStudentApplication, application.StudentApplicationStatus);
                return null;
            }

            // Проверяем, что отзыв ещё не оставлен
            var existing = await _context.PracticeReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdStudentApplication == dto.IdStudentApplication);
            if (existing != null)
            {
                _logger.LogWarning("Review already exists for application {Id}", dto.IdStudentApplication);
                return null;
            }

            var review = new PracticeReview
            {
                IdStudentApplication = dto.IdStudentApplication,
                Comment = dto.Comment,
                ReadyToWork = dto.ReadyToWork,
                SpecialityRelevance = dto.SpecialityRelevance,
                SupervisionQuality = dto.SupervisionQuality,
                ExperienceUsefulness = dto.ExperienceUsefulness,
                OverallScore = dto.OverallScore,
                CreatedAt = DateTime.Now
            };

            _context.PracticeReviews.Add(review);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Review {Id} created for application {AppId}", review.IdPracticeReview, dto.IdStudentApplication);
            return MapToResponseDto(review, application.Student);
        }

        // ========== Обновление отзыва ==========
        public async Task<PracticeReviewResponseDto?> UpdateAsync(int reviewId, UpdatePracticeReviewDto dto)
        {
            var review = await _context.PracticeReviews
                .Include(r => r.StudentApplication)
                .ThenInclude(a => a!.Student)
                .FirstOrDefaultAsync(r => r.IdPracticeReview == reviewId);
            if (review == null) return null;

            review.Comment = dto.Comment;
            review.ReadyToWork = dto.ReadyToWork;
            review.SpecialityRelevance = dto.SpecialityRelevance;
            review.SupervisionQuality = dto.SupervisionQuality;
            review.ExperienceUsefulness = dto.ExperienceUsefulness;
            review.OverallScore = dto.OverallScore;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Review {Id} updated", reviewId);
            return MapToResponseDto(review, review.StudentApplication?.Student);
        }

        // ========== Получение по ID ==========
        public async Task<PracticeReviewResponseDto?> GetByIdAsync(int reviewId)
        {
            var review = await _context.PracticeReviews
                .AsNoTracking()
                .Include(r => r.StudentApplication)
                .ThenInclude(a => a!.Student)
                .FirstOrDefaultAsync(r => r.IdPracticeReview == reviewId);
            if (review == null) return null;
            return MapToResponseDto(review, review.StudentApplication?.Student);
        }

        // ========== Получение по заявке ==========
        public async Task<PracticeReviewResponseDto?> GetByApplicationAsync(int studentApplicationId)
        {
            var review = await _context.PracticeReviews
                .AsNoTracking()
                .Include(r => r.StudentApplication)
                .ThenInclude(a => a!.Student)
                .FirstOrDefaultAsync(r => r.IdStudentApplication == studentApplicationId);
            if (review == null) return null;
            return MapToResponseDto(review, review.StudentApplication?.Student);
        }

        // ========== Все отзывы студента ==========
        public async Task<List<PracticeReviewResponseDto>> GetByStudentAsync(int studentId)
        {
            var reviews = await _context.PracticeReviews
                .AsNoTracking()
                .Include(r => r.StudentApplication)
                .ThenInclude(a => a!.Student)
                .Where(r => r.StudentApplication!.IdStudent == studentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return reviews.Select(r => MapToResponseDto(r, r.StudentApplication?.Student)).ToList();
        }

        // ========== Все отзывы (для внешних систем) ==========
        public async Task<List<PracticeReviewResponseDto>> GetAllAsync()
        {
            var reviews = await _context.PracticeReviews
                .AsNoTracking()
                .Include(r => r.StudentApplication)
                .ThenInclude(a => a!.Student)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return reviews.Select(r => MapToResponseDto(r, r.StudentApplication?.Student)).ToList();
        }

        // ========== Удаление ==========
        public async Task<bool> DeleteAsync(int reviewId)
        {
            var review = await _context.PracticeReviews.FirstOrDefaultAsync(r => r.IdPracticeReview == reviewId);
            if (review == null) return false;
            _context.PracticeReviews.Remove(review);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Review {Id} deleted", reviewId);
            return true;
        }

        // ========== Маппинг ==========
        private PracticeReviewResponseDto MapToResponseDto(PracticeReview review, Student? student)
        {
            return new PracticeReviewResponseDto
            {
                IdPracticeReview = review.IdPracticeReview,
                IdStudentApplication = review.IdStudentApplication,
                StudentName = student != null ? $"{student.Surname} {student.Name} {student.Patronymic}".Trim() : null,
                Comment = review.Comment,
                ReadyToWork = review.ReadyToWork,
                SpecialityRelevance = review.SpecialityRelevance,
                SupervisionQuality = review.SupervisionQuality,
                ExperienceUsefulness = review.ExperienceUsefulness,
                OverallScore = review.OverallScore,
                CreatedAt = review.CreatedAt
            };
        }
    }
}