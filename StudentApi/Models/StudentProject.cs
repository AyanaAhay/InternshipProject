using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("StudentProject")]
public class StudentProject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudentProject { get; set; }

    [Required]
    public int IdQuestionnaire { get; set; }

    [ForeignKey(nameof(IdQuestionnaire))]
    public virtual Questionnaire? Questionnaire { get; set; }

    [Required]
    [MaxLength(255)]
    public string ProjectName { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime? DateParticipation { get; set; }

    [MaxLength(255)]
    public string? Organizer { get; set; }

    public bool IsOurOrganizationEvent { get; set; }
}