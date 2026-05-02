using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("Education")]
public class Education
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int IdEducation { get; set; }

	[Required]
	public int IdQuestionnaire { get; set; }

	[ForeignKey(nameof(IdQuestionnaire))]
	public virtual Questionnaire? Questionnaire { get; set; }

	[Required]
	[MaxLength(255)]
	public string DegreeOfEducation { get; set; } = string.Empty;

	[Required]
	[MaxLength(255)]
	public string EducationalInstitution { get; set; } = string.Empty;

	[MaxLength(255)]
	public string? Faculty { get; set; }

	[MaxLength(255)]
	public string? Specialization { get; set; }

    // НОВОЕ - курс обучения
    [Required]
    public int? CourseNumber { get; set; } 

    [Column(TypeName = "date")]
	public DateTime? EducationStartDate { get; set; }

	[Column(TypeName = "date")]
	public DateTime? EducationEndDate { get; set; }

	[MaxLength(255)]
	public string? GroupNumber { get; set; }

	[MaxLength(255)]
	public string? SurnameTutor { get; set; }

	[MaxLength(255)]
	public string? NameTutor { get; set; }

	[MaxLength(255)]
	public string? PatronymicTutor { get; set; }
}