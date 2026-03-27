using System;

namespace PersonalProject.Models;

public class ProfileIndexViewModels
{
    public List<Skill> Skill { get; set; } = new List<Skill>();
    public List<Movie> Movie { get; set; } = new List<Movie>();
    public List<PortfolioItem> Portfolio { get; set; } = new List<PortfolioItem>();

    public string? CacheData { get; set; }
}
