using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("PsychologicalQuestions")]
public class PsychologicalQuestions
{
    [Key]
    [ForeignKey(nameof(Questionnaire))]
    public int IdQuestionnaire { get; set; }

    [Column(TypeName = "text")]
    public string? LateInstances { get; set; }

    [Column(TypeName = "text")]
    public string? ValuedQualities { get; set; }

    [Column(TypeName = "text")]
    public string? UnacceptableQualities { get; set; }

    [Column(TypeName = "text")]
    public string? Friendliness { get; set; }

    [Column(TypeName = "text")]
    public string? SubordinateAction { get; set; }

    [Column(TypeName = "text")]
    public string? WorkTimeDedication { get; set; }

    [Column(TypeName = "text")]
    public string? StressfulWorkReadiness { get; set; }

    [Column(TypeName = "text")]
    public string? DisciplineImportance { get; set; }

    public virtual Questionnaire? Questionnaire { get; set; }
}