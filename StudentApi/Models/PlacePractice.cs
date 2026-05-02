using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("PlacePractice")]
public class PlacePractice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPlacePractice { get; set; }

    [Required]
    public int IdQuestionnaire { get; set; }

    [ForeignKey(nameof(IdQuestionnaire))]
    public virtual Questionnaire? Questionnaire { get; set; }

    [Required]
    [MaxLength(255)]
    public string OrganizationName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(255)]
    public string? PhoneNumber { get; set; }

    [Column(TypeName = "date")]
    public DateTime? PracticeStartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? PracticeEndDate { get; set; }

    [Column(TypeName = "text")]
    public string? MainFunctions { get; set; }

    // НОВОЕ - обратная связь по практике
    [Column(TypeName = "text")] 
    public string? Feedback { get; set; } 
}