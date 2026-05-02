using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("PlaceWork")]
public class PlaceWork
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPlaceWork { get; set; }

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
    public DateTime? WorkStartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? WorkEndDate { get; set; }

    [MaxLength(255)]
    public string? Position { get; set; }

    [Column(TypeName = "text")]
    public string? MainFunctions { get; set; }

    [Column(TypeName = "text")]
    public string? ReasonForDismissal { get; set; }
}