using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("Relative")]
public class Relative
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdRelative { get; set; }

    [Required]
    public int IdQuestionnaire { get; set; }

    [ForeignKey(nameof(IdQuestionnaire))]
    public virtual Questionnaire? Questionnaire { get; set; }

    [Required]
    [MaxLength(255)]
    public string RelationDegree { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Patronymic { get; set; }

    [Column(TypeName = "date")]
    public DateTime? Birthdate { get; set; }

    [Column(TypeName = "text")]
    public string? PlaceStudy { get; set; }

    [Column(TypeName = "text")]
    public string? PlaceWork { get; set; }
}