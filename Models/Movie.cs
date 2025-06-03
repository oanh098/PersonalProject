using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalProject.Models;

public class Movie
{
    public int Id { get; set; }
    
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title { get; set; }

    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    
    [StringLength(30)]
    [RegularExpression("^[A-Z]+[a-zA-Z]*$", ErrorMessage = "Please enter a genre.")]
    [Required]
    public string? Genre { get; set; }
    public decimal? Price { get; set; }
    
    [RegularExpression("^[A-Z]+[a-zA-Z]*$", ErrorMessage = "Please enter a genre.")]
    [StringLength(5)]
    [Required]
    public string? Rating { get; set; }
}
