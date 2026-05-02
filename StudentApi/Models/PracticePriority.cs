using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("PracticePriority")]
public class PracticePriority
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPracticePriority { get; set; }

    [Required]
    public int IdQuestionnaire { get; set; }

    [ForeignKey(nameof(IdQuestionnaire))]
    public virtual Questionnaire? Questionnaire { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string Wording { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int Estimation { get; set; }
}