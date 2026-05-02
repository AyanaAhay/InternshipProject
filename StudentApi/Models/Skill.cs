using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApi.Models;

[Table("Skill")]
public class Skill
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSkill { get; set; }

    [Required]
    public int IdQuestionnaire { get; set; }

    [ForeignKey(nameof(IdQuestionnaire))]
    public virtual Questionnaire? Questionnaire { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string SkillName { get; set; } = string.Empty;
}