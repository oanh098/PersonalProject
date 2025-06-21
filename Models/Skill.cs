using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalProject.Models;

public class Skill
{
    public List<Skill> Skills { get; set; } = new List<Skill>(); 
    

     

    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Skill name must be between 2 and 100 characters.")]
    public string? Name { get; set; }

    [Required]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties can be added if needed for relationships
    // public ICollection<ProfileSkill> ProfileSkills { get; set; }

}
