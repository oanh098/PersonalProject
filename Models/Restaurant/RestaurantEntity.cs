using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalProject.Models.Restaurant;

public class RestaurantEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CuisineType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties can be added if needed for relationships
    public ICollection<RestaurantMenu>? Menus { get; set; } 
}
