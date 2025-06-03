using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PersonalProject.Models;

public class MovieGenreViewModel
{
    public List<Movie> Movies { get; set; } = new List<Movie>();
    public SelectList? Genres;
    public string? MovieGenre { get; set; }
    public string? SearchString { get; set; }
}
